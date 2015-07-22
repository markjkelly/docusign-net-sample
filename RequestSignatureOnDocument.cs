using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using DocuSignSample.DocuSign_Objects;
using Newtonsoft.Json;

namespace DocuSignAPIWalkthrough04
{
    public class RequestSignatureOnDocument
    {
        public static void Main()
        {
            //---------------------------------------------------------------------------------------------------
            // ENTER VALUES FOR THE FOLLOWING VARIABLES:
            //---------------------------------------------------------------------------------------------------
            string username = "";			// your account email
            string password = "!";			// your account password
            string integratorKey = "";			// your account Integrator Key (found on Preferences -> API page)
            string documentName = "Try DocuSigning.docx";			// copy document with same name and extension into project directory (i.e. "test.pdf")
            string contentType = "application/pdf";		// default content type is PDF

            string baseURL = "https://demo.docusign.net/restapi/v2/accounts/"; //Add your DocuSign account number to the end of the url

            //---------------------------------------------------------------------------------------------------

            try
            {

                /*
                    This is the only DocuSign API call that requires a "multipart/form-data" content type.  We will be 
                    constructing a request body in the following format (each newline is a CRLF):

                    --AAA
                    Content-Type: application/xml
                    Content-Disposition: form-data

                    <XML BODY GOES HERE>
                    --AAA
                    Content-Type:application/pdf
                    Content-Disposition: file; filename="document.pdf"; documentid=1 

                    <DOCUMENT BYTES GO HERE>
                    --AAA--
                 */

                // append "/envelopes" to baseURL and use for signature request api call
                string url = baseURL + "/envelopes";

                Envelope envelope = new Envelope();

                envelope.status = "sent";
                envelope.emailSubject = "Test API Call Create Envelope";

                Signer signer = new Signer();
                signer.email = "";      // Add recipient email
                signer.name = "";       // Add recipient name
                signer.recipientId = 1;
                
                SignHereTab signHereTab = new SignHereTab();
                signHereTab.anchorString = "/S1Sign/";
                signHereTab.anchorXOffset = "-20";
                signHereTab.anchorYOffset = "120";

                InitialHereTab initialHereTab = new InitialHereTab();
                initialHereTab.anchorString = "/S1Initial/";
                initialHereTab.anchorXOffset = "10";
                initialHereTab.anchorYOffset = "120";

                FullNameTab fullNameTab = new FullNameTab();
                fullNameTab.anchorString = "/S1FullName/";
                fullNameTab.anchorXOffset = "-20";
                fullNameTab.anchorYOffset = "120";

                DateSignedTab dateSignedTab = new DateSignedTab();
                dateSignedTab.anchorString = "/S1Date/";
                dateSignedTab.anchorXOffset = "-20";
                dateSignedTab.anchorYOffset = "120";

                signer.tabs = new Tabs();
                signer.tabs.signHereTabs = new List<SignHereTab>();
                signer.tabs.signHereTabs.Add(signHereTab);

                signer.tabs.initialHereTabs = new List<InitialHereTab>();
                signer.tabs.initialHereTabs.Add(initialHereTab);

                signer.tabs.fullNameTabs = new List<FullNameTab>();
                signer.tabs.fullNameTabs.Add(fullNameTab);

                signer.tabs.dateSignedTabs = new List<DateSignedTab>();
                signer.tabs.dateSignedTabs.Add(dateSignedTab);

                envelope.recipients = new Recipients();
                envelope.recipients.signers = new List<Signer>();
                envelope.recipients.signers.Add(signer);

                Document document = new Document();
                document.name = documentName;
                document.documentId = 1;

                envelope.documents = new List<Document>();
                envelope.documents.Add(document);

                string body = JsonConvert.SerializeObject(envelope);

                Trace.WriteLine(body);

                // set request url, method, headers.  Don't set the body yet, we'll set that seperately after
                // we read the document bytes and configure the rest of the multipart/form-data request
                HttpWebRequest request = initializeRequest(url, "POST", null, username, password, integratorKey);

                // some extra config for this api call
                configureMultiPartFormDataRequest(request, body, documentName, contentType);

                // read the http response
                string response = getResponseBody(request);

                //--- display results
                Trace.WriteLine("response: " + response);
            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    //Trace.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    {
                        string text = new StreamReader(data).ReadToEnd();
                        Trace.WriteLine(prettyPrintXml(text));
                    }
                }
            }
        } // end main()

        //***********************************************************************************************
        // --- HELPER FUNCTIONS ---
        //***********************************************************************************************
        public static HttpWebRequest initializeRequest(string url, string method, string body, string email, string password, string intKey)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            addRequestHeaders(request, email, password, intKey);
            if (body != null)
                addRequestBody(request, body);
            return request;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void addRequestHeaders(HttpWebRequest request, string email, string password, string intKey)
        {
            // authentication header can be in JSON or XML format.  XML used for this walkthrough:
            string authenticateStr =
                "<DocuSignCredentials>" +
                    "<Username>" + email + "</Username>" +
                    "<Password>" + password + "</Password>" +
                    "<IntegratorKey>" + intKey + "</IntegratorKey>" +
                    "</DocuSignCredentials>";
            request.Headers.Add("X-DocuSign-Authentication", authenticateStr);
            request.Accept = "application/json";
            request.ContentType = "application/json";
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
        public static void configureMultiPartFormDataRequest(HttpWebRequest request, string xmlBody, string docName, string contentType)
        {
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
        public static string parseDataFromResponse(string response, string searchToken)
        {
            // look for "searchToken" in the response body and parse its value
            using (XmlReader reader = XmlReader.Create(new StringReader(response)))
            {
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == searchToken))
                        return reader.ReadString();
                }
            }
            return null;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string prettyPrintXml(string xml)
        {
            // print nicely formatted xml
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception)
            {
                return xml;
            }
        }
    } // end class
} // end namespace