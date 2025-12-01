namespace KeepieScannerAgent.Models
{
    public class UploadAttachmentRequest
    {
        public string ClientId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // Base64
    }

    public class UploadAttachmentResponse
    {
        public bool Success { get; set; }
        public string AttachmentId { get; set; } = string.Empty;
    }
}
