# docusign-net-sample

Sample DocuSign .NET project based on this [code walkthrough](http://iodocs.docusign.com/APIWalkthrough/requestSignatureFromDocument).

The code from the original walkthrough has been modified:
- Anchor Text used for positioning of DocuSign tabs 
- JSON Request Format
- PONOs to construct DocuSign request body
- [Json.NET](https://github.com/JamesNK/Newtonsoft.Json) Package to serialize and deserialize request and response strings. 

In order to use this sample code, signup for a free DocuSign Developer account [here](https://secure.docusign.com/signup/developer) and add your credentials to the RequestSignatureOnDocument.cs class.

Resources Used:
1. [DocuSign API Explorer](http://iodocs.docusign.com/)
2. [Postman](https://www.getpostman.com/)
3. [json2csharp](http://json2csharp.com/)
