using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using DocuSignSample.DocuSign_Objects;
using Newtonsoft.Json;
using DocuSignSample;

namespace DocuSignAPIWalkthrough04
{
    public class RequestSignatureOnDocument
    {
        public static void Main()
        {
            DocuSignCredentials credentials = new DocuSignCredentials();
            credentials.username = "";                                   // your account email
            credentials.password = "";			                                    // your account password
            credentials.integratorKey = "";			// your account Integrator Key (found on Preferences -> API page)
            credentials.accountId = "";
            credentials.baseUrl = "https://demo.docusign.net/";

            DocuSignClient client = new DocuSignClient(credentials);

            Envelope envelope = new Envelope();

            envelope.status = "sent";
            envelope.emailSubject = "Test API Call Create Envelope";

            Signer signer = new Signer();
            signer.email = "";
            signer.name = "";
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
            document.name = "Try DocuSigning.docx";     // copy document with same name and extension into project directory (i.e. "test.pdf")
            document.documentId = 1;

            envelope.documents = new List<Document>();
            envelope.documents.Add(document);

            CreateEnvelopeResponse response = client.CreateAndSendEnvelope(envelope);

            Trace.WriteLine(response);

        }
    }
}