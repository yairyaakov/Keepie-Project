public interface IQueryExecutor
{
    List<Customer> Execute(string sqlQuery);
}
