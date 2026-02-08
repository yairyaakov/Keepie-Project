# Keepie – Candidate Home Assignment

## WhatsApp Broadcast Module + Local Scanner Agent

This repository contains two complete backend assignments for Keepie:

1. **WhatsApp Broadcast Module** – Sends WhatsApp messages generated from an SQL-style query.  
2. **Scanner Agent** – Local document scanning from the browser and uploading files to Keepie attachments.

---

# Task 1 – WhatsApp Broadcast Module

## Overview

The module receives:

- SQL query  
- Message template  
- WhatsApp provider configuration  

It then:

1. Executes the SQL query  
2. Extracts the relevant customer phone numbers  
3. Sends a personalized WhatsApp message to each customer through a pluggable provider  

Designed to be **clean, modular, and easily replaceable (plug-and-play)**.

---

## Project Structure

```text
KeepieWhatsAppModule/
│   Program.cs
│   Customer.cs
│   IQueryExecutor.cs
│   SimpleSqlQueryExecutor.cs
│   InMemoryQueryExecutor.cs
│   MessageDispatcher.cs
│   IWhatsAppProvider.cs
│   TwilioWhatsAppProvider.cs
│   WhatsAppConfig.cs
```

## Architecture Components

### 1. QueryExecutor Layer
Responsible for interpreting SQL-like text and returning matching customers.

**Two implementations included:**
- **InMemoryQueryExecutor** – simple filtering
- **SimpleSqlQueryExecutor** – supports `Active` filter + `CustomerName` filter

### 2. WhatsAppProvider Layer
Defines a pluggable interface:

```csharp
public interface IWhatsAppProvider
{
    Task SendMessageAsync(string phoneNumber, string messageText);
}
```

**Implementation included:**
- **TwilioWhatsAppProvider** – simulated HTTP POST request

The provider is fully **replaceable**, e.g.:
- Twilio  
- Meta Cloud API  
- Vonage

---

### 3. MessageDispatcher
Core business logic:

- Executes SQL query  
- Iterates through results  
- Formats message: `"Hi {name}, ..."`  
- Calls the WhatsApp provider  
- Logs **SUCCESS** / **FAILURE**

```csharp
public async Task ExecuteWhatsAppBroadcast(string sqlQuery, string messageTemplate)
```

### 4. Configuration
`WhatsAppConfig` injects the API URL and API key.  
No secrets are hard-coded inside logic classes.

---

### 5. Entry Point
`Program.cs`:

- Builds the customer list (mock database)  
- Chooses a `QueryExecutor` implementation  
- Instantiates the WhatsApp provider  
- Runs `ExecuteWhatsAppBroadcast(...)`

### How to Run

```bash
dotnet run
```

Or run with your own parameters:

```csharp
dotnet run -- "SELECT CustomerName, Phone, Active FROM Customers WHERE Active = 1 AND CustomerName = 'Ori'" "Hi {name}, sent from real SQLite!"
```

# Task 2 – Local Scanner Agent + Web Component

## Overview

This project implements a **local Scanner Agent** that listens on `http://localhost:9977` and allows the browser to:

1. Trigger a scan from a physical (or simulated) scanner  
2. Receive the scanned PDF in Base64 format  
3. Upload the scanned document to the Keepie server (mocked for this assignment)  
4. Optionally download the scanned file locally  

The project demonstrates a clean, modular architecture using dependency injection (DI), interfaces, separation of concerns, and replaceable components.

---

# 🏗 Architecture Overview

The system contains **three logical components**, all running locally for the purpose of the task:

            ┌──────────────────────────┐
            │        Browser           │
            │      (scan.js UI)        │
            └─────────────┬────────────┘
                          │
                          │ 1. GET /scan
                          ▼
            ┌──────────────────────────┐
            │        Local Agent       │
            │   ASP.NET Core (.NET 7) │
            │ ─ IScanner (Mock)       │
            │ ─ IFileEncoder (Base64) │
            │ ─ IScanService          │
            └─────────────┬────────────┘
                          │
                          │ 2. POST /api/attachments/upload
                          ▼
            ┌──────────────────────────┐
            │     Mock Keepie Server   │
            │ (inside same Program.cs) │
            │ returns {attachmentId}   │
            └──────────────────────────┘

Although the Keepie server is mocked, the design simulates a real-world scenario:

- **Agent**: Talks to the scanner  
- **Browser**: Initiates actions and displays results  
- **Keepie server**: Stores attachments  

---

# How the Agent Works

The Agent is implemented as a lightweight HTTP server using **ASP.NET Core Minimal APIs**.

### Key responsibilities:
- Expose a `GET /scan` endpoint  
- Trigger a scan using an `IScanner` implementation  
- Convert the scanned PDF into Base64 using `IFileEncoder`  
- Return a structured JSON object back to the browser  

### Scan flow:

```csharp
GET /scan
↓
IScanner.ScanAsync() // returns byte[]
↓
IFileEncoder.EncodeToBase64(byte[]) // returns Base64 string
↓
ScanResult { fileName, base64 }
```

### Technologies used:
- C# / .NET 7
- Minimal APIs
- Dependency Injection (DI)
- Local static file hosting (index.html + scan.js)

The Agent is **singleton-injected**, stateless, and modular.

---

# How the Browser Communicates With the Agent

The browser loads the UI from `wwwroot`:

- `index.html`
- `scan.js`

### When the user clicks **“Scan a document”**:

```js
const response = await fetch("/scan");
const scanResult = await response.json();
```
The browser receives:

```csharp
{
  "fileName": "scan_20250105_123456.pdf",
  "base64": "JVBERi0xLjUK..."
}
```

The Base64 string represents the entire PDF.

The browser:

- Displays the JSON  
- Stores it in memory (`lastScanResult`)  
- Enables the **Download** button  
- Automatically uploads it to the Keepie server mock  


---

### How the Upload to Attachments Works

After a successful scan, the browser calls:

POST /api/attachments/upload

With a JSON body:

```csharp
{
  "clientId": "123",
  "fileName": "scan_20250105_123456.pdf",
  "content": "<Base64 content>"
}
```

The mock Keepie API returns:

```csharp
{
  "success": true,
  "attachmentId": "c9a4bf2e-7f53-4a31-9eb5-0ef318af9def"
}
```

Why this matters:

It simulates the real production flow:

Real Scanner → Local Agent → Browser → Keepie Cloud

## Why the Solution Is Modular

The project uses **clean dependency injection (DI)** and **interfaces**, allowing each component to be replaced or upgraded independently.

### 🔹 Interfaces Used in the Architecture

- `IScanner` — defines how scanning is performed  
- `IFileEncoder` — defines how file bytes are encoded  
- `IScanService` — coordinates scanning + encoding logic  

### 🔹 Replaceable Components

| Interface       | Default Implementation | Purpose                                        |
|-----------------|------------------------|------------------------------------------------|
| `IScanner`      | `MockScanner`          | Can be replaced with a real hardware scanner   |
| `IFileEncoder`  | `Base64FileEncoder`    | Can encode using other strategies (ZIP, etc.)  |
| `IScanService`  | `ScanService`          | Can add logging, caching, transformations      |

### 🔹 What Can Be Replaced Easily?

Because each part is abstracted behind an interface, you can replace:

- The scanner (e.g., from `MockScanner` to a real scanner driver)  
- The encoder (Base64 → compression, encryption, etc.)  
- The upload target (local mock → real Keepie cloud API)  
- The storage mechanism  
- The UI or frontend framework  

All **without touching** other components in the system.

This is exactly the goal of **modular architecture**: each part is isolated and replaceable.

---

## Replaceable / Extendable Components

### Replace `MockScanner` with a real scanner driver

For example:

```csharp
builder.Services.AddSingleton<IScanner, EpsonScanner>();
```

### Replace Base64 encoder with ZIP/PDF manipulation library

```csharp
builder.Services.AddSingleton<IFileEncoder, PdfCompressor>();
```

### Replace the mock upload endpoint with a real Keepie API
Change:

```csharp
POST /api/attachments/upload
```

To:

```csharp
https://api.keepie.com/attachments/upload
```

### Improve UI (React/Vue/Angular)

The browser side is entirely decoupled.

### Add authentication to upload

(e.g., JWT, OAuth2)

## Project Structure

```text
KeepieScannerAgent/
│
├── Program.cs                 # Agent server + Mock Keepie API
│
├── Models/
│   ├── ScanResult.cs
│   ├── UploadAttachmentRequest.cs
│   └── UploadAttachmentResponse.cs
│
├── Services/
│   ├── IScanner.cs
│   ├── MockScanner.cs
│   ├── IFileEncoder.cs
│   ├── Base64FileEncoder.cs
│   ├── IScanService.cs
│   └── ScanService.cs
│
└── wwwroot/
    ├── index.html
    └── scan.js
```

## Architecture

The solution is split into clear, independent layers:

### 1. Backend – Scanner Agent (ASP.NET Core)

- **Program.cs**
  - Configures DI:
    - `IScanner` → `MockScanner`
    - `IFileEncoder` → `Base64FileEncoder`
    - `IScanService` → `ScanService`
  - Exposes endpoints:
    - `GET /scan` – perform scan and return JSON `{ fileName, base64 }`
    - `POST /api/attachments/upload` – mock Keepie attachment API
  - Serves static files from `wwwroot` (HTML, JS).

- **Services**
  - `IScanner` / `MockScanner`  
    Reads `sample.pdf` from disk (simulating a real hardware scanner).
  - `IFileEncoder` / `Base64FileEncoder`  
    Converts `byte[]` → Base64 string.
  - `IScanService` / `ScanService`  
    Orchestrates scanning + encoding and returns a `ScanResult`.

- **Models**
  - `ScanResult`  
    `{ string FileName, string Base64 }`
  - `UploadAttachmentRequest`  
    `{ string ClientId, string FileName, string Content }`
  - `UploadAttachmentResponse`  
    `{ bool Success, string AttachmentId }`

### 2. Frontend – Browser UI (HTML + JavaScript)

- **wwwroot/index.html**
  - Simple page with:
    - `Scan` button
    - `Download last PDF` button
    - Areas for status and raw JSON

- **wwwroot/scan.js**
  - On **Scan** click:
    - `GET /scan` → receives `{ fileName, base64 }`
    - Stores result in `lastScanResult`
    - Enables **Download** button
    - Calls `uploadAttachment(...)` to:
      - `POST /api/attachments/upload` with `{ clientId, fileName, content }`
  - On **Download last PDF** click:
    - Decodes Base64 → bytes → `Blob("application/pdf")`
    - Creates a temporary `<a>` and triggers a browser download.

### 3. High-Level Flow

```text
User clicks "Scan"
      ↓
Browser → GET /scan
      ↓
Scanner Agent:
  IScanner (MockScanner) → reads sample.pdf
  IFileEncoder (Base64FileEncoder) → bytes → Base64
  IScanService → returns ScanResult as JSON
      ↓
Browser:
  - Shows JSON
  - Saves lastScanResult
  - Calls POST /api/attachments/upload
      ↓
Mock Keepie Upload API:
  - Logs request
  - Returns { success, attachmentId }
      ↓
Browser:
  - Shows upload status
  - Optional: user clicks "Download last PDF" → saves PDF locally
```

## Summary

This project demonstrates a full **end-to-end local scanning flow** for the Keepie platform:

- Browser triggers the scan  
- The Agent processes it  
- PDF is encoded to Base64  
- The file is uploaded to the attachments service  
- The user can download the file locally  
- The architecture is highly modular and easily replaceable  

### ✔ The solution meets all task requirements:

- Local Agent  
- Browser–Agent communication  
- Upload flow  
- Modular design  
- Extensible architecture  




