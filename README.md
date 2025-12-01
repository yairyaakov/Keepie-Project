# Keepie Scanner Agent – Architecture & Workflow

This document provides a concise technical overview of the Keepie Scanner Agent, how it interacts with the browser, and how scanned documents are uploaded to Keepie attachments.  
It also explains why the solution is modular and which components can be replaced or improved.

---

## 1. How the Agent Works

The Keepie Scanner Agent is a lightweight local web service built with **ASP.NET Core**.  
It runs on the user’s machine and provides an HTTP interface for scanning documents.

In this exercise, the scanner is **mocked**:

- `MockScanner` simulates a physical scanner by reading a local sample PDF.
- `ScanService` converts the scanned bytes to Base64 and returns a structured result.
- The endpoint `GET /scan` triggers a scan and returns:

```json
{
  "fileName": "scan_YYYYMMDD_HHMMSS.pdf",
  "base64": "<PDF Base64>"
}














Keepie Scanner Agent – Architecture Overview
1. How the Agent Works

The Keepie Scanner Agent is a lightweight local service running on the end-user’s machine.
Its purpose is to provide a secure, simple bridge between a physical scanner and the web browser, without requiring the browser to access the hardware directly.

🔹 Core Responsibilities

Expose a local HTTP endpoint (GET /scan)

Trigger a scan operation (via a mock scanner in this assignment)

Convert the scanned file (PDF bytes) into Base64

Return a structured JSON response containing:

fileName

base64 (the scanned document)

🔹 Internal Components

The Agent is built using dependency injection and clear interfaces:

Interface	Responsibility
IScanner	Handles the actual scan process (mocked by reading sample.pdf)
IFileEncoder	Converts binary files into Base64 strings
IScanService	Orchestrates scanning + encoding into a single workflow
🔹 Scan Workflow (Step-by-Step)

Browser calls GET /scan

The Agent resolves IScanService via DI

IScanService:

Calls IScanner.ScanAsync()
→ returns raw PDF bytes

Passes the bytes to IFileEncoder
→ returns Base64

A ScanResult object is created

The Agent sends it back to the browser as JSON

🔹 Why Base64?

Binary files cannot be transferred in a JSON HTTP response.
Base64 provides a safe text-based representation of the PDF.
