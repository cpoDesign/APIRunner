// See https://aka.ms/new-console-template for more information

namespace APITestingRunner
{
  internal class TestRunner
  {
    private Configuration.Config _config;
    private IEnumerable<DataQueryResult>? _dbBasedItems = new List<DataQueryResult>();
    private readonly List<string> _errors = new();
    private readonly List<string> responses = new();
    public TestRunner()
    {
    }

    internal TestRunner ApplyConfig(Configuration.Config? configSettings)
    {
      if (configSettings == null)
      {
        _errors.Add("Failed to load configuration");
      }

      _config = configSettings;
      return this;
    }

    internal async Task<TestRunner> GetTestRunnerDbSet()
    {
      // connect to database and load information
      DataAccess db = new(_config);
      _dbBasedItems = await db.FetchDataForRunnerAsync();

      if (_dbBasedItems != null)
      {
        Console.WriteLine($"Found {_dbBasedItems.Count()}");
      }

      return this;
    }

    internal async Task<TestRunner> RunTestsAsync()
    {
      // create a request to the api

      HttpClient client = new()
      {
        BaseAddress = new Uri(_config.UrlBase)
      };

      if (_config.HeaderParam != null && _config.HeaderParam.Count > 0)
      {
        foreach (Configuration.Param item in _config.HeaderParam)
        {
          client.DefaultRequestHeaders.Add(item.Name, item.value);
        }
      }

      if (_dbBasedItems.Count() > 0)
      {
        //http://localhost:5152/Data?id=1
        foreach (DataQueryResult item in _dbBasedItems)
        {
          await MakeApiCorCollectionCall(client, item);
        }
      }
      else
      {
        //http://localhost:5152/Data?id=1
        await MakeApiCall(client);
      }

      return this;
    }

    private async Task MakeApiCall(HttpClient client)
    {
      switch (_config.RequestType)
      {
        case "GET":

          HttpResponseMessage request = await client.GetAsync(ComposeRequest(null));

          string response = $"{request.StatusCode} - {await request.Content.ReadAsStringAsync()}";
          responses.Add(response);
          Console.Write(response);

          break;
        case "POST":
        case "PUT":
        case "DELETE":
        default:
          _errors.Add("unsupported request type");
          break;
      }
    }

    private async Task MakeApiCorCollectionCall(HttpClient client, DataQueryResult item)
    {
      switch (_config.RequestType)
      {
        case "GET":

          HttpResponseMessage request = await client.GetAsync(ComposeRequest(item));

          string response = $"{request.StatusCode} - {await request.Content.ReadAsStringAsync()}";
          responses.Add(response);
          Console.WriteLine(response);

          break;
        case "POST":
        case "PUT":
        case "DELETE":
        default:
          _errors.Add("unsupported request type");
          break;
      }
    }

    internal string? ComposeRequest(DataQueryResult? dbData)
    {
      string arguments = $"{_config.UrlPath}?";
      foreach (Configuration.Param item in _config.UrlParam)
      {
        arguments += $"{item.Name}=";

        //check if item value is listed in dbfields, if yes we have mapping to value from database otherwise  just use value
        if (_config.DBFields.Any(x => x.value == item.value) && dbData != null)
        {
          //replace value from dbData object
          KeyValuePair<string, string> dbResultFound = dbData.Results.FirstOrDefault(x => x.Key == item.value);

          arguments += $"{dbResultFound.Value}";

        }
        else
        {
          // no match found in parameters 
          arguments += $"{item.value}";
        }

        arguments += "&";
      }

      return arguments;
    }

    internal async Task<TestRunner> PrintResults()
    {
      foreach (string error in _errors)
      {
        Console.WriteLine(error);
      }

      await Task.CompletedTask;
      return this;
    }
  }
}