// See https://aka.ms/new-console-template for more information

using System.Net.Mime;
using System.Text;

namespace APITestingRunner
{
  internal class TestRunner
  {
    private Config _config;
    private IEnumerable<DataQueryResult>? _dbBasedItems = new List<DataQueryResult>();
    private readonly List<string> _errors = new();
    private readonly List<string> responses = new();
    private readonly List<TestResultStatus> _resultsStats = new();
    private HttpClient? compareClient = null;
    private string? compareUrl;

    public TestRunner()
    {
    }

    internal async Task ApplyConfig(Config? configSettings)
    {
      if (configSettings == null)
      {
        _errors.Add("Failed to load configuration");
      }

      _config = configSettings;
      await Task.CompletedTask;
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


      if (!string.IsNullOrWhiteSpace(_config.CompareUrlBase))
      {
        compareClient = new()
        {
          BaseAddress = new Uri(_config.UrlBase)
        };
      }

      if (_config.HeaderParam != null && _config.HeaderParam.Count > 0)
      {
        foreach (ConfigurationManager.Param item in _config.HeaderParam)
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
        url = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, null);

        if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
        {
          compareUrl = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.CompareUrlPath, _config, null);
        }
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
        url = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, item);
        if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
        {
          compareUrl = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.CompareUrlPath, _config, item);
        }
      }
      catch (Exception)
      {
        _errors.Add($"Error has occurred while composing an url: {url}");
        return;
      }

      await MakeApiCorCollectionCall(client, url, item);


      return;
    }

    private async Task MakeApiCorCollectionCall(HttpClient client, string url, DataQueryResult? item = null, string requestBody = "")
    {
      HttpResponseMessage response;
      try
      {
        switch (_config.RequestType)
        {
          case ConfigurationManager.RequestType.GET:

            if (!string.IsNullOrWhiteSpace(requestBody))
            {

              HttpRequestMessage request = new()
              {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Content = new StringContent(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json /* or "application/json" in older versions */),
              };

              response = await client.SendAsync(request);
            }
            else
            {
              response = await client.GetAsync(url);
            }

            string content = await response.Content.ReadAsStringAsync();


            if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
            {
              List<string> compareList = new();

              if (compareClient != null)
              {
                HttpResponseMessage compareResponse = await compareClient.GetAsync(compareUrl);

                string responseCompareContent = await response.Content.ReadAsStringAsync();


                // compare status code
                if (response.StatusCode == compareResponse.StatusCode)
                {
                  compareList.Add($"Status code SourceAPI: {response.StatusCode} CompareAPI: {response.StatusCode}");
                }

                // compare content
                if (content != responseCompareContent)
                {
                  compareList.Add("APIs content does not match");
                }

                if (compareList.Count == 0)
                {
                  Console.ForegroundColor = ConsoleColor.Green;
                  Console.WriteLine($"Comparing API for {item?.RowId} success");
                }
                else
                {
                  Console.Write($"Comparing API for {item?.RowId} Failed");
                  foreach (string errorsInComparrison in compareList)
                  {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"- {errorsInComparrison}");
                  }
                }

                Console.ForegroundColor = ConsoleColor.White;

                _ = ProcessResultCapture(new ApiCallResult(compareResponse.StatusCode, responseCompareContent, compareResponse.Headers, url, item, compareResponse.IsSuccessStatusCode), true);
              }
              else
              {
                _errors.Add("Failed to find configuration for compare API");
              }
            }

            await ProcessResultCapture(new ApiCallResult(response.StatusCode, content, response.Headers, url, item, response.IsSuccessStatusCode));

            break;
          case ConfigurationManager.RequestType.POST:
          case ConfigurationManager.RequestType.PUT:
          case ConfigurationManager.RequestType.DELETE:
          default:
            _errors.Add("Unsupported request type");
            break;
        }
      }
      catch (Exception ex)
      {
        _errors.Add($"Error has occurred while calling api with url:{url} with message: {ex.Message}");
      }
    }

    private async Task ProcessResultCapture(ApiCallResult apiCallResult, bool IsCompareFile = false)
    {
      string response = $"{apiCallResult.statusCode} - {apiCallResult.responseContent}";


      if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.Capture)
      {
        if (_config.ResultsStoreOption == ConfigurationManager.StoreResultsOption.All || (_config.ResultsStoreOption == ConfigurationManager.StoreResultsOption.FailuresOnly && !apiCallResult.IsSuccessStatusCode))
        {
          if (_config.LogLocation != null)
          {
            await logIntoFileAsync(_config.LogLocation, apiCallResult, IsCompareFile);
          }
          else
          {
            _errors.Add("No logLocation found");
          }
        }
      }

      if (!IsCompareFile)
      {
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
      }

      Console.WriteLine(response);
    }

    private async Task logIntoFileAsync(string logLocation, ApiCallResult apiCallResult, bool IsCompareFile)
    {
      string filePrefix = "request";
      try
      {
        string resultsDirectory = Path.Combine(logLocation, "Results");
        if (!Directory.Exists(resultsDirectory))
        {
          _ = Directory.CreateDirectory(resultsDirectory);
        }

        string fileName = $"{filePrefix}-{apiCallResult.item?.RowId}";
        if (IsCompareFile)
        {
          fileName += "Compare";
        }

        await new FileOperations().WriteFile(resultsDirectory, $"{fileName}.json", apiCallResult);
      }
      catch
      {
      }
    }

    internal async Task<TestRunner> PrintResults()
    {
      Console.WriteLine("==========Status==========");
      foreach (TestResultStatus item in _resultsStats)
      {
        Console.WriteLine($"{item.StatusCode} - Count: {item.NumberOfResults}");
      }

      if (_errors.Count > 0)
      {
        Console.WriteLine("==========Errors==========");
        foreach (string error in _errors)
        {
          Console.WriteLine(error);
        }
      }

      await Task.CompletedTask;
      return this;
    }
  }
}