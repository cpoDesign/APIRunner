// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Http.Headers;

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

          HttpResponseMessage response = await client.GetAsync(ComposeRequest(item));
          string responseContent = $"{response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
          ProcessResultCapture(response.StatusCode, responseContent, response.Headers);

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

          HttpResponseMessage response = await client.GetAsync(ComposeRequest(item));
          string responseContent = $"{response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
          ProcessResultCapture(response.StatusCode, responseContent, response.Headers);


          break;
        case "POST":
        case "PUT":
        case "DELETE":
        default:
          _errors.Add("unsupported request type");
          break;
      }
    }

    private void ProcessResultCapture(HttpStatusCode statusCode, string responseContent, HttpResponseHeaders headers)
    {
      string response = $"{statusCode} - {responseContent}";


      TestResultStatus? existingResult = _resultsStats.FirstOrDefault(x => x.StatusCode == (int)statusCode);
      if (existingResult == null)
      {
        _resultsStats.Add(new TestResultStatus { StatusCode = (int)statusCode, NumberOfResults = 1 });
      }
      else
      {
        existingResult.NumberOfResults++;
      }

      responses.Add(response);

      Console.WriteLine(response);
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

      Console.WriteLine("============Status==========");
      foreach (TestResultStatus item in _resultsStats)
      {
        Console.WriteLine($"{item.StatusCode} - Count: {item.NumberOfResults}");
      }

      Console.WriteLine("============Errors==========");
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