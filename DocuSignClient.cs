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
            Trace.WriteLine("url: " + url);
            string requestBody = JsonConvert.SerializeObject(envelope);

            HttpWebRequest request = initializeRequest(url, "POST", requestBody, "application/json", docusignCredentials.username, docusignCredentials.password, docusignCredentials.integratorKey);
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
    }
}