using System.Threading.Tasks;

public interface IWhatsAppProvider
{
    Task SendMessageAsync(string phoneNumber, string messageText);
}
