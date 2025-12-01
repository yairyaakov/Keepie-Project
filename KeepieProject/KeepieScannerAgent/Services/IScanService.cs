using System.Threading.Tasks;
using KeepieScannerAgent.Models;

namespace KeepieScannerAgent.Services
{
    public interface IScanService
    {
        Task<ScanResult> ScanToBase64Async();
    }
}
