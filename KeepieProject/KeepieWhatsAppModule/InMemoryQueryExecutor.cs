public class InMemoryQueryExecutor : IQueryExecutor
{
    private readonly List<Customer> _customers;

    public InMemoryQueryExecutor(List<Customer> customers)
    {
        _customers = customers;
    }

    public List<Customer> Execute(string sqlQuery)
    {
        Console.WriteLine($"[QueryExecutor] Executing query: {sqlQuery}");

        if (sqlQuery.Contains("WHERE") && sqlQuery.Contains("Active = 1"))
        {
            return _customers.Where(c => c.Active).ToList();
        }

        if (sqlQuery.Contains("WHERE") && sqlQuery.Contains("Active = 0"))
        {
            return _customers.Where(c => !c.Active).ToList();
        }

        return _customers;
    }
}
