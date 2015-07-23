using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using Newtonsoft.Json;
using DocuSignSample.DocuSign_Objects;

namespace DocuSignSample
{
    public class DocuSignClient
    {
        public DocuSignCredentials docusignCredentials;

        public DocuSignClient(DocuSignCredentials docusignCredentials)
        {
            this.docusignCredentials = docusignCredentials;
        }

        public CreateEnvelopeResponse CreateAndSendEnvelope(Envelope envelope)
        {
            Trace.WriteLine("Entering DocuSignClient.CreateAndSendEnvelope()");
            string url = docusignCredentials.baseUrl + "restapi/v2/accounts/" + docusignCredentials.accountId + "/envelopes";
            string requestBody = JsonConvert.SerializeObject(envelope);

            // set request url, method, headers.  Don't set the body yet, we'll set that seperately after
            // we read the document bytes and configure the rest of the multipart/form-data request
            HttpWebRequest request = initializeRequest(url, "POST", null, "application/json", docusignCredentials.username, docusignCredentials.password, docusignCredentials.integratorKey);

            // some extra config for this api call
            configureMultiPartFormDataRequest(request, requestBody, "Try DocuSigning.docx", "application/pdf");

            CreateEnvelopeResponse createEnvelopeResponse = JsonConvert.DeserializeObject<CreateEnvelopeResponse>(getResponseBody(request));

            Trace.WriteLine("DocuSign Response: [" + JsonConvert.SerializeObject(createEnvelopeResponse) + "]");

            Trace.WriteLine("Exiting DocuSignClient.CreateDraftEnvelopeFromTemplateOnBehalfOf()");
            return createEnvelopeResponse;
        }


        //***********************************************************************************************
        // --- HELPER FUNCTIONS ---
        //***********************************************************************************************
        public static HttpWebRequest initializeRequest(string url, string method, string body, string contentType, string email, string password, string intKey)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            addRequestHeaders(request, contentType, email, password, intKey);
            if (body != null)
                addRequestBody(request, body);
            return request;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static HttpWebRequest initializeRequest(string url, string method, string body, string contentType, string email, string password, string intKey, string authorization)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            addRequestHeaders(request, contentType, email, password, intKey, authorization);
            if (body != null)
                addRequestBody(request, body);
            return request;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static HttpWebRequest initializeRequest(string url, string method, string body, string contentType, string email, string password, string intKey, string authorization, string actAsUser)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            addRequestHeaders(request, contentType, email, password, intKey, authorization, actAsUser);
            if (body != null)
                addRequestBody(request, body);
            return request;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void addRequestHeaders(HttpWebRequest request, string contentType, string email, string password, string intKey)
        {
            string authenticateStr =
                "<DocuSignCredentials>" +
                    "<Username>" + email + "</Username>" +
                    "<Password>" + password + "</Password>" +
                    "<IntegratorKey>" + intKey + "</IntegratorKey>" +
                    "</DocuSignCredentials>";
            request.Headers.Add("X-DocuSign-Authentication", authenticateStr);
            request.Accept = "application/json";
            request.ContentType = contentType;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void addRequestHeaders(HttpWebRequest request, string contentType, string email, string password, string intKey, string authorization)
        {
            string authenticateStr =
                "<DocuSignCredentials>" +
                    "<Username>" + email + "</Username>" +
                    "<Password>" + password + "</Password>" +
                    "<IntegratorKey>" + intKey + "</IntegratorKey>" +
                    "</DocuSignCredentials>";
            request.Headers.Add("X-DocuSign-Authentication", authenticateStr);
            request.Headers.Add("Authorization", authorization);
            request.Accept = "application/json";
            request.ContentType = contentType;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void addRequestHeaders(HttpWebRequest request, string contentType, string email, string password, string intKey, string authorization, string actAsUser)
        {
            string authenticateStr =
                "<DocuSignCredentials>" +
                    "<Username>" + email + "</Username>" +
                    "<Password>" + password + "</Password>" +
                    "<IntegratorKey>" + intKey + "</IntegratorKey>" +
                    "</DocuSignCredentials>";
            request.Headers.Add("X-DocuSign-Authentication", authenticateStr);
            request.Headers.Add("Authorization", authorization);
            request.Headers.Add("X-DocuSign-Act-As-User", actAsUser);
            request.Accept = "application/json";
            request.ContentType = contentType;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void addRequestBody(HttpWebRequest request, string requestBody)
        {
            // create byte array out of request body and add to the request object
            byte[] body = System.Text.Encoding.UTF8.GetBytes(requestBody);
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(body, 0, requestBody.Length);
            dataStream.Close();
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string getResponseBody(HttpWebRequest request)
        {
            // read the response stream into a local string
            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream());
            string responseText = sr.ReadToEnd();
            return responseText;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void configureMultiPartFormDataRequest(HttpWebRequest request, string xmlBody, string docName, string contentType)
        {
            /*
            This is the only DocuSign API call that requires a "multipart/form-data" content type.  We will be 
            constructing a request body in the following format (each newline is a CRLF):

            --AAA
            Content-Type: application/json
            Content-Disposition: form-data

            <REQUEST BODY GOES HERE>
            --AAA
            Content-Type:application/pdf
            Content-Disposition: file; filename="document.pdf"; documentid=1 

            <DOCUMENT BYTES GO HERE>
            --AAA--
            */

            // overwrite the default content-type header and set a boundary marker
            request.ContentType = "multipart/form-data; boundary=BOUNDARY";

            // start building the multipart request body
            string requestBodyStart = "\r\n\r\n--BOUNDARY\r\n" +
                "Content-Type: application/json\r\n" +
                    "Content-Disposition: form-data\r\n" +
                    "\r\n" +
                    xmlBody + "\r\n\r\n--BOUNDARY\r\n" + 	// our xml formatted envelopeDefinition
                    "Content-Type: " + contentType + "\r\n" +
                    "Content-Disposition: file; filename=\"" + docName + "\"; documentId=1\r\n" +
                    "\r\n";
            string requestBodyEnd = "\r\n--BOUNDARY--\r\n\r\n";

            Trace.WriteLine("File.Exists? " + docName + " [" + File.Exists(docName) + "]");
            // read contents of provided document into the request stream
            FileStream fileStream = File.OpenRead(docName);


            // write the body of the request
            byte[] bodyStart = System.Text.Encoding.UTF8.GetBytes(requestBodyStart.ToString());
            byte[] bodyEnd = System.Text.Encoding.UTF8.GetBytes(requestBodyEnd.ToString());
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(bodyStart, 0, requestBodyStart.ToString().Length);

            // Read the file contents and write them to the request stream.  We read in blocks of 4096 bytes
            byte[] buf = new byte[4096];
            int len;
            while ((len = fileStream.Read(buf, 0, 4096)) > 0)
            {
                dataStream.Write(buf, 0, len);
            }
            dataStream.Write(bodyEnd, 0, requestBodyEnd.ToString().Length);
            dataStream.Close();
        }
    }
}