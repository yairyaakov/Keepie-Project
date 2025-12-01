using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class SimpleSqlQueryExecutor : IQueryExecutor
{
    private readonly List<Customer> _customers;

    public SimpleSqlQueryExecutor(List<Customer> customers)
    {
        _customers = customers;
    }

    public List<Customer> Execute(string sqlQuery)
    {
        Console.WriteLine($"[SimpleSqlQueryExecutor] Executing query: {sqlQuery}");

        IEnumerable<Customer> query = _customers;

        var normalized = sqlQuery.ToUpperInvariant();

        bool? activeFilter = null;

        if (normalized.Contains("WHERE"))
        {
            if (normalized.Contains("ACTIVE = 1"))
            {
                activeFilter = true;
            }
            else if (normalized.Contains("ACTIVE = 0"))
            {
                activeFilter = false;
            }
        }

        if (activeFilter.HasValue)
        {
            query = query.Where(c => c.Active == activeFilter.Value);
        }

        // ---- 2. סינון לפי שם לקוח, לדוגמה:
        // WHERE Active = 1 AND CustomerName = 'Ori'
        var nameMatch = Regex.Match(
            sqlQuery,
            "CustomerName\\s*=\\s*'([^']+)'",
            RegexOptions.IgnoreCase
        );

        if (nameMatch.Success)
        {
            var requestedName = nameMatch.Groups[1].Value;
            query = query.Where(c =>
                string.Equals(c.CustomerName, requestedName, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }
}
