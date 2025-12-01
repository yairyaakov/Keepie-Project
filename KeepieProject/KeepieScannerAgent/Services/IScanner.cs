using System.Threading.Tasks;

namespace KeepieScannerAgent.Services
{
    public interface IScanner
    {
        Task<byte[]> ScanAsync();
    }
}
