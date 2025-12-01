using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 1. Mock DB
        var customers = new List<Customer>
        {
            new Customer { CustomerName = "Ori",  Phone = "+972500000001", Active = true  },
            new Customer { CustomerName = "Dana", Phone = "+972500000002", Active = true  },
            new Customer { CustomerName = "Neta", Phone = "+972500000003", Active = false }
        };

        // 2. Twilio configuration
        // In a real system these values would come from:
        // - appsettings.json
        // - environment variables
        // - secret manager
        // For this assignment we hard-code them.
        var twilioConfig = new WhatsAppConfig
        {
            ApiUrl = "https://api.twilio.com/2010-04-01/Accounts/ACXXXXXXXXXXXXXXXXX/Messages.json",
            ApiKey = "TWILIO_AUTH_TOKEN_PLACEHOLDER"
        };

        // 3. Build module components
        //IQueryExecutor queryExecutor = new InMemoryQueryExecutor(customers);
        IQueryExecutor queryExecutor = new SimpleSqlQueryExecutor(customers);
        IWhatsAppProvider provider = new TwilioWhatsAppProvider(twilioConfig);
        var dispatcher = new MessageDispatcher(queryExecutor, provider);

        string sqlQuery;
        string message;

        // 4. If user passed SQL + message as command-line arguments, use them.
        if (args.Length >= 2)
        {
            sqlQuery = args[0];
            message  = args[1];
        }
        else
        {
            // Default SQL query example
            sqlQuery = @"
                SELECT CustomerName, Phone
                FROM Customers
                WHERE Active = 1 AND CustomerName = 'Ori'
            ";

            // Alternative examples:
            //sqlQuery = @"
            //    SELECT CustomerName, Phone
            //    FROM Customers
            //    WHERE Active = 0
            //";

            //sqlQuery = @"
            //    SELECT CustomerName, Phone
            //    FROM Customers
            //    WHERE CustomerName = 'Ori'
            //";

            // Default message template
            message = "Hi {name}, this is a WhatsApp message from Keepie via Twilio!";
        }

        // 6. Execute broadcast
        await dispatcher.ExecuteWhatsAppBroadcast(sqlQuery, message);
    }
}
