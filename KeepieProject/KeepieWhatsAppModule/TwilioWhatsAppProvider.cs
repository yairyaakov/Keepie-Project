using System;
using System.Threading.Tasks;

// This class simulates a Twilio WhatsApp provider.
// It demonstrates that the provider does NOT hold API keys internally,
// but receives them through configuration (Dependency Injection).
// This shows that the implementation is modular and easily replaceable.
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

        // Authentication header (Basic Auth) - simulated
        Console.WriteLine($"Header: Authorization: Basic <{_config.ApiKey}>");

        // Simulated request body structure for Twilio WhatsApp API
        Console.WriteLine("Body:");
        Console.WriteLine($"    To: whatsapp:{phoneNumber}");
        Console.WriteLine("    From: whatsapp:+14155238886");
        Console.WriteLine($"    Body: {messageText}");
        Console.WriteLine("==========================================");

        // No real HTTP request is sent — this is only a simulation.
        return Task.CompletedTask;
    }
}
