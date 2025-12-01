using KeepieScannerAgent.Models;
using KeepieScannerAgent.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services for DI
builder.Services.AddSingleton<IScanner, MockScanner>();
builder.Services.AddSingleton<IFileEncoder, Base64FileEncoder>();
builder.Services.AddSingleton<IScanService, ScanService>();

var app = builder.Build();

// Simple logging middleware for all incoming requests
app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Incoming request: {Method} {Path}",
        context.Request.Method,
        context.Request.Path);

    await next();
});

// Serve index.html + static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// GET /scan – calls the Agent to "scan" and return Base64
app.MapGet("/scan", async (IScanService scanService) =>
{
    app.Logger.LogInformation("Handling /scan – starting scan");

    ScanResult result = await scanService.ScanToBase64Async();

    app.Logger.LogInformation("Scan finished. FileName={FileName}, Base64Length={Length}",
        result.FileName,
        result.Base64.Length);

    return Results.Json(result);
});

// Mock API: POST /api/attachments/upload
app.MapPost("/api/attachments/upload", (UploadAttachmentRequest request) =>
{
    app.Logger.LogInformation(
        "Received upload request. ClientId={ClientId}, FileName={FileName}, ContentLength={Length}",
        request.ClientId,
        request.FileName,
        request.Content?.Length ?? 0);

    var response = new UploadAttachmentResponse
    {
        Success = true,
        AttachmentId = Guid.NewGuid().ToString()
    };

    app.Logger.LogInformation("Returning upload response. AttachmentId={AttachmentId}",
        response.AttachmentId);

    return Results.Json(response);
});

// Listen on http://localhost:9977
app.Run("http://localhost:9977");
