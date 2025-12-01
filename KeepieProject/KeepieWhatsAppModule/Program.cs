using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 1. DB מדומה
        var customers = new List<Customer>
        {
            new Customer { CustomerName = "Ori", Phone = "+972500000001", Active = true },
            new Customer { CustomerName = "Dana", Phone = "+972500000002", Active = true },
            new Customer { CustomerName = "Neta", Phone = "+972500000003", Active = false }
        };

        // 2. Twilio Config 
        // In a real system, these values would come from:
        // - appsettings.json
        // - environment variables
        // - secret manager
        // For this assignment, values are hard-coded.
        
        var twilioConfig = new WhatsAppConfig
        {
            ApiUrl = "https://api.twilio.com/2010-04-01/Accounts/ACXXXXXXXXXXXXXXXXX/Messages.json",
            ApiKey = "TWILIO_AUTH_TOKEN_PLACEHOLDER"
        };

        // 3. בניית הרכיבים
        //IQueryExecutor queryExecutor = new InMemoryQueryExecutor(customers);
        IQueryExecutor queryExecutor = new SimpleSqlQueryExecutor(customers);
        IWhatsAppProvider provider = new TwilioWhatsAppProvider(twilioConfig);
        var dispatcher = new MessageDispatcher(queryExecutor, provider);

        string sqlQuery;
        string message;

        if (args.Length >= 2)
        {
            sqlQuery = args[0];
            message  = args[1];
        }
        else
        {
            // 4. השאילתה לדוגמה
            sqlQuery = @"
                SELECT CustomerName, Phone
                FROM Customers
                WHERE Active = 1 AND CustomerName = 'Ori'
            ";

            //string sqlQuery = @"
            //    SELECT CustomerName, Phone
            //    FROM Customers
            //    WHERE Active = 0
            //";

            //string sqlQuery = @"
            //    SELECT CustomerName, Phone
            //    FROM Customers
            //    WHERE CustomerName = 'Ori'
            //";


            // 5. הודעה לדוגמה
            message = "Hi {name}, this is a WhatsApp message from Keepie via Twilio!";  
        }
        

        // 6. הרצה
        await dispatcher.ExecuteWhatsAppBroadcast(sqlQuery, message);
    }
}
