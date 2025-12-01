using System;
using System.Threading.Tasks;

public class MessageDispatcher
{
    private readonly IQueryExecutor _queryExecutor;
    private readonly IWhatsAppProvider _whatsAppProvider;

    public MessageDispatcher(IQueryExecutor queryExecutor, IWhatsAppProvider whatsAppProvider)
    {
        _queryExecutor = queryExecutor;
        _whatsAppProvider = whatsAppProvider;
    }

    public async Task ExecuteWhatsAppBroadcast(string sqlQuery, string messageTemplate)
    {
        var customers = _queryExecutor.Execute(sqlQuery);

        foreach (var customer in customers)
        {
            try
            {
                string personalizedMessage =
                    messageTemplate.Replace("{name}", customer.CustomerName);

                await _whatsAppProvider.SendMessageAsync(customer.Phone, personalizedMessage);

                Console.WriteLine($"[SUCCESS] Sent message to {customer.CustomerName} ({customer.Phone})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FAILURE] Could not send to {customer.CustomerName} ({customer.Phone}).");
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
