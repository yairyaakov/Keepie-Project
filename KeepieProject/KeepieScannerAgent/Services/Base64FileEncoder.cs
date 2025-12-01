using System;

namespace KeepieScannerAgent.Services
{
    public class Base64FileEncoder : IFileEncoder
    {
        public string EncodeToBase64(byte[] fileBytes)
        {
            return Convert.ToBase64String(fileBytes);
        }
    }
}
