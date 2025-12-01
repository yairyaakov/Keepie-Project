namespace KeepieScannerAgent.Services
{
    public interface IFileEncoder
    {
        string EncodeToBase64(byte[] fileBytes);
    }
}
