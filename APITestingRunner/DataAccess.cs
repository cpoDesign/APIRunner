using Dapper;
using Microsoft.Data.SqlClient;

namespace APITestingRunner
{
  public class DataAccess
  {
    private readonly Config config;

    public DataAccess(Config config)
    {
      this.config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task<IEnumerable<DataQueryResult>> FetchDataForRunnerAsync()
    {
      if (string.IsNullOrWhiteSpace(config.DBConnectionString)) throw new TestRunnerConfigurationErrorsException("Failed to load connection string");
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
        foreach (ConfigurationManager.Param config in config.DBFields)
        {
          object sum = fields[config.Name];
          resultItem.Results.Add(new KeyValuePair<string, string>(config.Name, sum.ToString()));
        }

        list.Add(resultItem);
      }

      return list;
    }
  }
}
