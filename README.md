Keepie Scanner Agent – Architecture & Workflow
1. How the Agent Works

The Keepie Scanner Agent is a lightweight local web service (ASP.NET Core) that runs on the user’s machine.
Its purpose is to communicate with the physical scanner and expose a simple HTTP API that the browser can call.

In this exercise the scanner itself is mocked:

MockScanner simulates a real scanner by reading a sample PDF from disk.

ScanService converts the scanned bytes into Base64 and wraps them in a response model.

The endpoint GET /scan triggers the scan and returns { fileName, base64 }.

The Agent does not store files. It simply:

Activates the scanner

Converts the scanned file to Base64

Returns it to the browser

2. How the Browser Communicates with the Agent

The browser loads a small web UI from the Agent (index.html + scan.js).
When the user clicks Scan, the browser performs:
fetch("/scan")  // GET request to the local agent
The Agent responds with JSON containing:
{
  "fileName": "...",
  "base64": "..."
}
This JSON response is kept in memory on the client side (lastScanResult),
so the browser can either upload it to Keepie or download it locally.
3. How the Upload to Attachments Is Performed

After scanning, the browser immediately sends the file to the (mocked) Keepie server:
fetch("/api/attachments/upload", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
        clientId: "...",
        fileName: scanResult.fileName,
        content: scanResult.base64
    })
});
This simulates the real production flow:

Agent → Browser → Keepie Server

The Keepie mock returns:
{
  "success": true,
  "attachmentId": "..."
}
In a real environment, this would be the moment where the file is stored in cloud storage and linked to the relevant client record.
4. Why the Solution Is Modular

The architecture follows clear separation of concerns:
| Layer / Component   | Responsibility                              |
| ------------------- | ------------------------------------------- |
| `IScanner`          | Describes how scanning is performed         |
| `MockScanner`       | Current mock implementation for local demo  |
| `IFileEncoder`      | Defines how bytes are encoded               |
| `Base64FileEncoder` | Encodes file bytes to Base64                |
| `IScanService`      | Business logic: scan + encode               |
| `ScanService`       | Implementation of scanning workflow         |
| API Endpoints       | HTTP interface for browser                  |
| `scan.js`           | Frontend logic calling the Agent and Keepie |

Because each responsibility is isolated behind interfaces, components can be replaced without touching the rest of the system.
This is the entire purpose of using Dependency Injection + Interfaces.

5. Parts That Can Be Replaced or Improved

The modular design allows the following improvements without breaking the app:

🔧 Replaceable / Pluggable components

Scanner implementation
Replace MockScanner with a real hardware integration.

Encoding strategy
Swap Base64FileEncoder with compression or alternative encoding.

Upload endpoint
Replace the mocked Keepie API with the real cloud Keepie service.

Storage logic
Currently no file is stored. In real production, the server would save files to Blob Storage / S3.

Authentication
Add JWT-based authentication, so clientId comes from a real user session.

UI
Replace the simple HTML/JS page with a production-level React/Angular component.

🧩 Independent layers

Each component can evolve separately because nothing is hard-coded into the others.

Summary

This project demonstrates the full workflow of:

Scanning → Returning Base64 → Uploading to Keepie → Downloading locally

It is built in a clean, modular, extensible architecture using:

Interfaces

Dependency Injection

Clear separation between Agent, Browser, and Keepie Server

making it suitable for real-world expansion.

If you want, I can format this directly as a README.md file with proper Markdown headers, code formatting, and GitHub styling.
