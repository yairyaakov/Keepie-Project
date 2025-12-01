using System;
using System.Threading.Tasks;
using KeepieScannerAgent.Models;

namespace KeepieScannerAgent.Services
{
    public class ScanService : IScanService
    {
        private readonly IScanner _scanner;
        private readonly IFileEncoder _encoder;

        public ScanService(IScanner scanner, IFileEncoder encoder)
        {
            _scanner = scanner;
            _encoder = encoder;
        }

        public async Task<ScanResult> ScanToBase64Async()
        {
            byte[] bytes = await _scanner.ScanAsync();
            string base64 = _encoder.EncodeToBase64(bytes);

            return new ScanResult
            {
                FileName = $"scan_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf",
                Base64 = base64
            };
        }
    }
}
