// See https://aka.ms/new-console-template for more information

namespace APITestingRunner
{
  internal class TestRunner
  {
    private Configuration.Config _config;
    private IEnumerable<DataQueryResult>? _dbBasedItems = new List<DataQueryResult>();
    private readonly List<string> _errors = new();
    private readonly List<string> responses = new();
    private readonly List<TestResultStatus> _resultsStats = new();

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
        Console.WriteLine($"Found {_dbBasedItems.Count()} records for test");
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
          Console.WriteLine($"proceeding with call for record {item.RowId}");

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
      string? url = string.Empty;
      try
      {
        url = ComposeRequest(null);
      }
      catch (Exception)
      {
        _errors.Add($"Error has occurred while composing an url: {url}");
        return;
      }

      await MakeApiCorCollectionCall(client, url);
      return;
    }

    private async Task MakeApiCorCollectionCall(HttpClient client, DataQueryResult item)
    {
      string? url = string.Empty;
      try
      {
        url = ComposeRequest(item);
      }
      catch (Exception)
      {
        _errors.Add($"Error has occurred while composing an url: {url}");
        return;
      }

      await MakeApiCorCollectionCall(client, url, item);


      return;
    }

    private async Task MakeApiCorCollectionCall(HttpClient client, string url, DataQueryResult? item = null)
    {
      try
      {
        switch (_config.RequestType)
        {
          case "GET":

            HttpResponseMessage response = await client.GetAsync(url);
            string responseContent = $"{response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
            ProcessResultCapture(new ApiCallResult(response.StatusCode, responseContent, response.Headers, url, item));

            break;
          case "POST":
          case "PUT":
          case "DELETE":
          default:
            _errors.Add("unsupported request type");
            break;
        }
      }
      catch (Exception ex)
      {
        _errors.Add($"Error occured while calling api with url{url} with message {ex.Message}");
      }
    }

    private async void ProcessResultCapture(ApiCallResult apiCallResult)
    {
      string response = $"{apiCallResult.statusCode} - {apiCallResult.responseContent}";
      if (_config.LogLocation != null)
      {

        logIntoFile(_config.LogLocation, apiCallResult);
      }

      TestResultStatus? existingResult = _resultsStats.FirstOrDefault(x => x.StatusCode == (int)apiCallResult.statusCode);
      if (existingResult == null)
      {
        _resultsStats.Add(new TestResultStatus { StatusCode = (int)apiCallResult.statusCode, NumberOfResults = 1 });
      }
      else
      {
        existingResult.NumberOfResults++;
      }

      responses.Add(response);
      await Task.CompletedTask;

      Console.WriteLine(response);
    }

    private void logIntoFile(string logLocation, ApiCallResult apiCallResult)
    {
      try
      {
        string resultsDirectory = Path.Combine(logLocation, "Results");
        if (!Directory.Exists(resultsDirectory))
        {
          _ = Directory.CreateDirectory(resultsDirectory);
        }

        _ = new FileOperations().WriteFile(resultsDirectory, $"request{apiCallResult.item?.RowId}", apiCallResult);
      }
      catch
      {
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
      Console.WriteLine("==========Status==========");
      foreach (TestResultStatus item in _resultsStats)
      {
        Console.WriteLine($"{item.StatusCode} - Count: {item.NumberOfResults}");
      }

      Console.WriteLine("==========Errors==========");
      foreach (string error in _errors)
      {
        Console.WriteLine(error);
      }

      await Task.CompletedTask;
      return this;
    }
  }

  public class TestResultStatus
  {
    public int StatusCode { get; set; }
    public int NumberOfResults { get; set; } = 0;
  };
}