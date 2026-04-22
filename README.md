# DocuSign .NET Sample — Request Signature on Document

A C# .NET console application demonstrating how to create and send a DocuSign envelope using the REST API directly via `HttpWebRequest`.

## Overview

This sample walks through the complete envelope creation flow: building a typed envelope model, attaching a document, placing anchor-text signature tabs, and submitting everything as a `multipart/form-data` POST to the DocuSign REST API. It is a useful reference for understanding the raw API surface before migrating to an official SDK, and for seeing how the `X-DocuSign-Authentication` header authentication scheme works in practice.

## Features

- REST envelope creation via `POST /restapi/v2/accounts/{accountId}/envelopes`
- Anchor-text tab placement — `SignHereTab`, `InitialHereTab`, `FullNameTab`, `DateSignedTab` — using string anchors embedded in the document
- Typed POCO models for all DocuSign request and response objects (`Envelope`, `Signer`, `Tabs`, `Document`, `Recipients`, `CreateEnvelopeResponse`)
- `multipart/form-data` request construction combining a JSON envelope definition with binary document bytes
- Json.NET (`Newtonsoft.Json`) serialization and deserialization of all API payloads
- `log4net` logging via `System.Diagnostics.Trace`

## Prerequisites

| Requirement | Notes |
|---|---|
| .NET Framework 4.5.1 | Targets `net451`; Visual Studio 2013 or later recommended |
| Visual Studio | Community edition is sufficient |
| NuGet | Package restore requires internet access on first build |
| DocuSign developer account | Free at [developers.docusign.com](https://developers.docusign.com) |

## Setup

1. Clone the repository:
   ```
   git clone https://github.com/markjkelly/docusign-net-sample.git
   ```
2. Open `DocuSignSample.sln` (or `DocuSignSample.csproj`) in Visual Studio.
3. Restore NuGet packages: right-click the solution → **Restore NuGet Packages**. This pulls `Newtonsoft.Json 6.0.8` and `log4net 2.0.3`.
4. Copy a document named `Try DocuSigning.docx` into the project root (next to `DocuSignSample.csproj`). The file name must match exactly — it is hardcoded in `DocuSignClient.cs`.
5. Fill in your credentials in `RequestSignatureOnDocument.cs` (see [Configuration](#configuration) below).

## Configuration

All configuration is set directly in `RequestSignatureOnDocument.cs`:

| Variable | Field | Description |
|---|---|---|
| `IntegratorKey` | `credentials.integratorKey` | API integrator key from **Preferences → API** in the DocuSign web app |
| `Email` | `credentials.username` | Your DocuSign account email address |
| `Password` | `credentials.password` | Your DocuSign account password |
| `AccountId` | `credentials.accountId` | Your numeric account ID (visible in the DocuSign URL after login) |
| `RecipientEmail` | `signer.email` | Email address of the person who will receive the signature request |
| `RecipientName` | `signer.name` | Display name shown to the recipient |

> ⚠️ **These credentials are hardcoded for sample purposes. In production, use environment variables or a secrets manager.**

The `baseUrl` defaults to `https://demo.docusign.net/` (sandbox). Switch to `https://www.docusign.net/` for production accounts.

## Running

Press **F5** in Visual Studio to build and run, or use MSBuild from the command line:

```
msbuild DocuSignSample.csproj /p:Configuration=Debug
bin\Debug\DocuSignSample.exe
```

On success, the application prints the `CreateEnvelopeResponse` (including the new `envelopeId`) to the debug output. The recipient will receive an email with a link to sign the document.

## Key Files

| File | Purpose |
|---|---|
| `DocuSignClient.cs` | HTTP client — constructs `HttpWebRequest` objects, sets `X-DocuSign-Authentication` headers, builds the multipart body, and reads the response |
| `RequestSignatureOnDocument.cs` | Entry point — configures credentials, builds the envelope model with tabs and a signer, calls `CreateAndSendEnvelope` |
| `DocuSign Objects/` | POCO model classes for every API object: `Envelope`, `Signer`, `Tabs`, `Document`, `Recipients`, `CreateEnvelopeResponse`, and all tab types |
| `App.config` | Targets .NET Framework 4.5.1 runtime; includes commented-out `System.Net` verbose trace listener config useful for debugging HTTP traffic |

## API Version Note

> Written against the DocuSign REST API v2 using legacy Header Authentication (`X-DocuSign-Authentication`). The DocuSign eSignature REST API now uses OAuth 2.0. See [developers.docusign.com](https://developers.docusign.com) for current SDKs and samples.

## License

MIT
