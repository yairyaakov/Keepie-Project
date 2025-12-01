using System;
using System.Threading.Tasks;

//
//זה משדר למראיין: "הבנתי שפונקציית השליחה לא אמורה לדעת את ה־Key בעצמה, אלא לקבל אותו מה־config".
public class TwilioWhatsAppProvider : IWhatsAppProvider
{
    private readonly WhatsAppConfig _config;

    public TwilioWhatsAppProvider(WhatsAppConfig config)
    {
        _config = config;
    }

    public Task SendMessageAsync(string phoneNumber, string messageText)
    {
        Console.WriteLine("========== TWILIO HTTP REQUEST ==========");
        Console.WriteLine($"[HTTP POST] {_config.ApiUrl}");

        Console.WriteLine($"Header: Authorization: Basic <{_config.ApiKey}>");

        Console.WriteLine("Body:");
        Console.WriteLine($"    To: whatsapp:{phoneNumber}");
        Console.WriteLine("    From: whatsapp:+14155238886");
        Console.WriteLine($"    Body: {messageText}");
        Console.WriteLine("==========================================");

        return Task.CompletedTask;
    }
}
