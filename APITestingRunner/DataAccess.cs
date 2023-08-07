using Dapper;
using Microsoft.Data.SqlClient;

namespace APITestingRunner
{
  internal class DataAccess
  {
    private readonly Configuration.Config config;

    public DataAccess(Configuration.Config config)
    {

      this.config = config ?? throw new ArgumentNullException(nameof(config));
    }
    public async Task<IEnumerable<DataQueryResult>> FetchDataForRunnerAsync()
    {
      using SqlConnection connection = new(config.DBConnectionString);

      IEnumerable<object> result = await connection.QueryAsync<object>(config.DBQuery);

      List<DataQueryResult> list = new();
      int i = 0;
      foreach (object rows in result)
      {
        i++;

        DataQueryResult resultItem = new() { RowId = i, Results = new List<KeyValuePair<string, string>>() };

        IDictionary<string, object>? fields = rows as IDictionary<string, object>;

        // get the fields from database and match to the object
        foreach (Configuration.Param config in config.DBFields)
        {
          object sum = fields[config.Name];
          resultItem.Results.Add(new KeyValuePair<string, string>(config.Name, sum.ToString()));
        }


        list.Add(resultItem);
      }


      return list;
    }
  }

  public class DataQueryResult
  {
    public int RowId { get; set; } = 0;
    public List<KeyValuePair<string, string>> Results = new();
  }
}
