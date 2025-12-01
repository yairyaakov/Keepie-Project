const scanBtn = document.getElementById("scanBtn");
const downloadBtn = document.getElementById("downloadBtn");
const output = document.getElementById("output");
const rawJson = document.getElementById("rawJson");

let lastScanResult = null; // will hold { fileName, base64 } from /scan

// Calls backend API to upload the scanned document (Base64) to the server
async function uploadAttachment(scanResult) {
    const payload = {
        clientId: "123",              // hard-coded for now
        fileName: scanResult.fileName,
        content: scanResult.base64    // Base64 content from the Agent
    };

    const response = await fetch("/api/attachments/upload", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(payload)
    });

    if (!response.ok) {
        throw new Error("Upload failed with status: " + response.status);
    }

    const data = await response.json();
    return data; // expected: { success: true, attachmentId: "..." }
}

// Converts Base64 string to a PDF file and triggers download in the browser
function downloadLastPdf() {
    if (!lastScanResult) {
        alert("No scan result available to download.");
        return;
    }

    // Decode Base64 to binary
    const base64 = lastScanResult.base64;
    const binaryString = atob(base64);
    const len = binaryString.length;
    const bytes = new Uint8Array(len);

    for (let i = 0; i < len; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }

    // Create a Blob from the bytes
    const blob = new Blob([bytes], { type: "application/pdf" });

    // Create a temporary link and trigger download
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = lastScanResult.fileName || "scan.pdf";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
}

scanBtn.addEventListener("click", async () => {
    output.textContent = "Scanning using local Agent...";

    try {
        // 1) Call the local Agent to scan and get Base64
        const scanResponse = await fetch("/scan");

        if (!scanResponse.ok) {
            output.textContent = "Error calling /scan: " + scanResponse.status;
            return;
        }

        const scanResult = await scanResponse.json();
        // scanResult should be: { fileName: "...", base64: "..." }

        // Save last scan result for download
        lastScanResult = scanResult;
        downloadBtn.disabled = false;

        // Show JSON nicely in the <pre> block
        rawJson.textContent = JSON.stringify(scanResult, null, 2);

        output.textContent =
            "Scanned file: " + scanResult.fileName + "\n" +
            "Base64 length: " + scanResult.base64.length + " characters";

        // 2) Upload the scanned document to the (mock) Keepie server
        output.textContent += "\n\nUploading attachment to server...";

        const uploadResult = await uploadAttachment(scanResult);

        if (uploadResult.success) {
            output.textContent +=
                "\nUpload succeeded. Attachment ID: " + uploadResult.attachmentId;
        } else {
            output.textContent += "\nUpload response indicates failure.";
        }
    } catch (err) {
        output.textContent = "Error: " + err;
    }
});

// Hook up the download button
downloadBtn.addEventListener("click", downloadLastPdf);
