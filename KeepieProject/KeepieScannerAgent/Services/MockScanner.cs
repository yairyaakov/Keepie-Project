using System.IO;
using System.Threading.Tasks;

namespace KeepieScannerAgent.Services
{
    public class MockScanner : IScanner
    {
        private const string SampleFileName = "sample.pdf";

        public Task<byte[]> ScanAsync()
        {
            if (!File.Exists(SampleFileName))
            {
                throw new FileNotFoundException(
                    $"Sample file '{SampleFileName}' was not found in the working directory.");
            }

            byte[] bytes = File.ReadAllBytes(SampleFileName);
            return Task.FromResult(bytes);
        }
    }
}
