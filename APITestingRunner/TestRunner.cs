// See https://aka.ms/new-console-template for more information

using APITestingRunner.ApiRequest;
using APITestingRunner.Configuration;
using APITestingRunner.Database;
using APITestingRunner.IoOperations;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace APITestingRunner
{
	/// <summary>
	/// Instance of test runner executing the tests.
	/// </summary>
	/// <param name="logger">Instance of a logger.</param>
	/// <exception cref="ArgumentNullException">Can throw when logger is not provided.</exception>
	public class TestRunner(ILogger logger)
	{
        private Config? _config;
		private readonly List<TestResultStatus> _resultsStats = [];
		private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

		public List<string> Errors { get; } = [];

		internal async Task ApplyConfig(Config? configSettings)
		{
            if (configSettings == null)
            {
                Errors.Add("Failed to load configuration");
            }
            else
            {
                _config = configSettings;
            }

            await Task.CompletedTask;
        }

        internal async Task<TestRunner> RunTestsAsync()
        {
			ArgumentNullException.ThrowIfNull(_config);

			// create a request to the api
			var handler = new HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, errors) =>
                            {
                                return true;
                            };

            //TODO: conver to HTTPFactory to produce api calls
            HttpClient client = new(handler)
            {
                BaseAddress = new Uri(_config.UrlBase)
            };

            PopulateClientHeadersFromConfig(client, _config.HeaderParam);

            await MakeApiCall(client);

            return this;
        }

        private void PopulateClientHeadersFromConfig(HttpClient client, List<Param> headerParam)
        {
			ArgumentNullException.ThrowIfNull(_config);

			if (headerParam != null && headerParam.Count > 0)
            {
                foreach (var item in _config.HeaderParam)
                {
                    client.DefaultRequestHeaders.Add(item.Name, item.Value);
                }
            }
        }

        private async Task MakeApiCall(HttpClient client)
        {
			ArgumentNullException.ThrowIfNull(_config);

			var numberOfResults = 0;
            foreach (var dataQueryResult in await GetDataToProcessAsync())
            {
                await MakeApiForCollectionCall(client, dataQueryResult, numberOfResults++, PopulateRequestBody(_config, dataQueryResult));
            }

            return;
        }

        public static string PopulateRequestBody(Config config, DataQueryResult dataQueryResult)
        {
			_ = config ?? throw new ArgumentNullException(nameof(config));
            _ = dataQueryResult ?? throw new ArgumentNullException(nameof(dataQueryResult));

            return ReplaceValueWithDataSource(config.RequestBody!, dataQueryResult);
        }

        public static string ReplaceValueWithDataSource(string stringToUpdate, DataQueryResult dataQueryResult)
        {
            if (!string.IsNullOrWhiteSpace(stringToUpdate))
            {
                // we have a value
                var replaceValues = stringToUpdate;

                if (dataQueryResult.Results.Count > 0)
                {
                    foreach (var item in dataQueryResult.Results)
                    {
                        replaceValues = replaceValues.Replace($"{{{item.Key}}}", item.Value);
                    }
                }

                return replaceValues;
            }


            return string.Empty;
        }

        /// <summary>
        /// Gets data for processing.
        /// TODO: convert to yield
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<IEnumerable<DataQueryResult>> GetDataToProcessAsync()
        {
			ArgumentNullException.ThrowIfNull(_config);

			_logger.LogInformation("Validating database based data source start");
            /// return data source
            if (!string.IsNullOrWhiteSpace(_config.DBConnectionString))
            {

                _logger.LogInformation("Found database connection string");

                if (!string.IsNullOrWhiteSpace(_config.DBQuery) && _config?.DBFields?.Count > 0)
                {

                    _logger.LogInformation("Found database query and db fields. Attempting to load data from database.");

                    return await new DataAccess(_config, _logger).FetchDataForRunnerAsync();
                }
            }

            return new List<DataQueryResult> { new() { RowId = 0 } };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client">Client contains base url</param>
        /// <param name="pathAndQuery">contains the relative paths</param>
        /// <param name="item"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task MakeApiForCollectionCall(HttpClient client, DataQueryResult item, int numberOfResults, string requestBody = "")
        {
			ArgumentNullException.ThrowIfNull(_config);

			HttpResponseMessage? response = null;
            var onScreenMessage = string.Empty;

            var pathAndQuery = string.Empty;
            try
            {
                pathAndQuery = new DataRequestConstructor()
                                    .AddBaseUrl(_config.UrlBase)
                                    .ComposeUrlAddressForRequest(_config.UrlPath, _config, item)
                                    .GetPathAndQuery();

                //if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
                //{
                //    compareUrl = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.CompareUrlPath, _config, null);
                //}
            }
            catch (Exception)
            {
                Errors.Add($"Error has occurred while composing an url: {pathAndQuery}");
                return;
            }

            if (string.IsNullOrWhiteSpace(pathAndQuery))
            {
                Errors.Add("Failed to compose path and query for API request");
                return;
            }

   //         //TODO Pav to fix: restore this functionality
			//try
			//{
			//	client.BaseAddress = new Uri(_config.UrlBase);
			//}catch (Exception)
			//{
			//	Errors.Add($"Failed to parse url base from config.UrlBase: {_config.UrlBase}");
			//}

            // update variables from url directly on url
            pathAndQuery = TestRunner.ReplaceValueWithDataSource(pathAndQuery, item);

            try
            {
                switch (_config.RequestType)
                {
                    case RequestType.GET:

                        if (string.IsNullOrWhiteSpace(requestBody))
                        {
                            // we are using only data in url query
                            response = await client.GetAsync($"{_config.UrlBase}{pathAndQuery}");
                        }
                        else
                        {
                            //TODO: there are people with existing APIs that are using get with providing data as part of request body we need to cater for.
                            throw new NotImplementedException();
                            //// we are sending data in body
                            //HttpRequestMessage request = new()
                            //{
                            //    Method = HttpMethod.Get,
                            //    Content = CreateRequestContent(requestBody)
                            //};

                            //response = await client.SendAsync(request);
                        }

                        break;
                    case RequestType.POST:

                        var requestContent = CreateRequestContent(requestBody);
                        Debug.WriteLine($"Capturing requestBody: {await requestContent.ReadAsStringAsync()}");
                        response = await client.PostAsync(pathAndQuery, requestContent);

                        break;
                    case RequestType.PUT:
                        response = await client.PutAsync(pathAndQuery, CreateRequestContent(requestBody));
                        break;
                    case RequestType.PATCH:
                        response = await client.PatchAsync(pathAndQuery, CreateRequestContent(requestBody));
                        break;
                    case RequestType.DELETE:
                        response = await client.DeleteAsync(pathAndQuery);
                        break;
                    default:
                        Errors.Add("Unsupported request type");
                        break;
                }

                Debug.WriteLine(response);

                if (response != null)
                {
                    _resultsStats.Add(new TestResultStatus()
                    {
                        NumberOfResults = numberOfResults,
                        StatusCode = (int)response.StatusCode
                    });
                    
                if (response != null) {
                    onScreenMessage = GenerateResponseMessage(_config.RequestType, pathAndQuery, response);
                    var content = await response.Content.ReadAsStringAsync();
                    var fileName = string.Empty;
                    var responseHeaders = response.Headers.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString() ?? string.Empty)).ToList();
                    ProcessingFileResult result = null!;
                    switch (_config.ConfigMode)
                    {
                        case TesterConfigMode.Run:
                            // no another work necessary here
                            break;

                        case TesterConfigMode.Capture:
                            fileName = TestRunner.GenerateResultName(item, _config.ResultFileNamePattern);

                            result = await ProcessResultCaptureAndCompareIfRequested(
                                new ApiCallResult(response.StatusCode, content, responseHeaders, pathAndQuery, item, response.IsSuccessStatusCode));
                            if (result.DisplayFilePathInLog)
                            {
                                onScreenMessage += $" {TestConstants.TestOutputDirectory}/{fileName}";
                            }
                            break;

                        case TesterConfigMode.CaptureAndCompare:
                            fileName = TestRunner.GenerateResultName(item, _config.ResultFileNamePattern);


                            result = await ProcessResultCaptureAndCompareIfRequested(new ApiCallResult(response.StatusCode, content, responseHeaders, pathAndQuery, item, response.IsSuccessStatusCode));

                            if (result.DisplayFilePathInLog)
                            {
                                onScreenMessage += $" {TestConstants.TestOutputDirectory}/{fileName}";
                            }
                            onScreenMessage += $" {Enum.GetName(result.ComparissonStatus)}";

                            break;

                        default:
                            Errors.Add("This option is currently not supported");
                            throw new NotImplementedException();
                    }
                    
                    PopulateResultsData(numberOfResults, response, "test");

                    //TODO:implement
                    ///case TesterConfigMode.APICompare:
                    //if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
                    //{
                    //    List<string> compareList = new();

                    //        string responseCompareContent = await response.Content.ReadAsStringAsync();


                    //        // compare status code
                    //        if (response.StatusCode == compareResponse.StatusCode)
                    //        {
                    //            compareList.Add($"Status code SourceAPI: {response.StatusCode} CompareAPI: {response.StatusCode}");
                    //        }

                    //        // compare content
                    //        if (content != responseCompareContent)
                    //        {
                    //            compareList.Add("APIs content does not match");
                    //        }

                    //        if (compareList.Count == 0)
                    //        {
                    //            Console.ForegroundColor = ConsoleColor.Green;
                    //            Console.WriteLine($"Comparing API for {item?.RowId} success");
                    //        }
                    //        else
                    //        {
                    //            Console.Write($"Comparing API for {item?.RowId} Failed");
                    //            foreach (string errorsInComparrison in compareList)
                    //            {
                    //                Console.ForegroundColor = ConsoleColor.Red;
                    //                Console.WriteLine($"- {errorsInComparrison}");
                    //            }
                    //        }

                    //        Console.ForegroundColor = ConsoleColor.White;

                    //        _ = ProcessResultCapture(new ApiCallResult(compareResponse.StatusCode, responseCompareContent, compareResponse.Headers, pathAndQuery, item, compareResponse.IsSuccessStatusCode), true);
                    //    }

                    //    await ProcessResultCapture(new ApiCallResult(response.StatusCode, content, response.Headers, pathAndQuery, item, response.IsSuccessStatusCode));
                    //}
                    //else
                    //{
                    //    _errors.Add("Failed to find configuration for compare API");
                    //}
                }
            }
            catch (Exception ex)
            {
                Errors.Add($"Error has occurred while calling api with url:{client.BaseAddress} with {pathAndQuery} with message:{onScreenMessage} && exception:  {ex.Message}");
            }
            finally
            {
                _logger.LogInformation(onScreenMessage);
            }
        }

    public void PopulateResultsData(int numberOfResults, HttpResponseMessage? response, string url) {

      if (response == null) {
        // response was not set so something went wrong adding only an error
        _errors.Add($"Failed to populate result for {url}");
        return;
      }

      var existingCode = _resultsStats.FirstOrDefault(x => x.StatusCode == (int)response.StatusCode);

      if (existingCode == null) {
        _resultsStats.Add(new TestResultStatus() {
          NumberOfResults = numberOfResults,
          StatusCode = (int)response.StatusCode
        });
      } else {
        existingCode.NumberOfResults += numberOfResults;
      }
    }

    public List<TestResultStatus> GetResults() {
      return _resultsStats;
    }

    public string GenerateResponseMessage(RequestType requestType, string relativeUrl, HttpResponseMessage? response) {
      var determination = "fail";

            if (response is not null)
            {
                if (response.IsSuccessStatusCode) determination = "success";
                return $"{requestType} {relativeUrl} {(int)response.StatusCode} {determination}";
            }

            return $"{relativeUrl} failed to return";
        }

        private static StringContent CreateRequestContent(string requestBody)
        {
            return new(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);
        }

        private async Task<ProcessingFileResult> ProcessResultCaptureAndCompareIfRequested(ApiCallResult apiCallResult)
        {
			ArgumentNullException.ThrowIfNull(_config);

			//TODO: Review what this is for?
			_ = $"{apiCallResult.StatusCode} - {apiCallResult.ResponseContent}";

            var fileCompareStatus = ComparissonStatus.NewFile;
            var result = new ProcessingFileResult { ComparissonStatus = fileCompareStatus };

            if (_config.ConfigMode == TesterConfigMode.Capture || _config.ConfigMode == TesterConfigMode.CaptureAndCompare)
            {
                if (_config.ResultsStoreOption == StoreResultsOption.All || (_config.ResultsStoreOption == StoreResultsOption.FailuresOnly && !apiCallResult.IsSuccessStatusCode))
                {
                    if (_config.OutputLocation != null)
                    {
                        result.DisplayFilePathInLog = true;
                        result.ComparissonStatus = await LogIntoFileAsync(_config.OutputLocation, apiCallResult);
                    }
                    else
                    {
                        Errors.Add("No logLocation found");
                    }
                }
            }


            //if (IsCompareFile) {
            //    TestResultStatus? existingResult = _resultsStats.FirstOrDefault(x => x.StatusCode == (int)apiCallResult.statusCode);
            //    if (existingResult == null) {
            //        _resultsStats.Add(new TestResultStatus { StatusCode = (int)apiCallResult.statusCode, NumberOfResults = 1 });
            //    } else {
            //        existingResult.NumberOfResults++;
            //    }

            //    responses.Add(response);
            //}

            return result;

        }

        /// <summary>
        /// Validate If File already exists.
        /// </summary>
        /// <param name="logLocation"></param>
        /// <param name="apiCallResult"></param>
        /// <param name="captureCompareFile"></param>
        /// <returns>Boolean result checking if file already exists.</returns>
        private async Task<ComparissonStatus> LogIntoFileAsync(string logLocation, ApiCallResult apiCallResult)
        {
			ArgumentNullException.ThrowIfNull(_config);

			try
            {
                var resultsDirectory = Path.Combine(logLocation, TestConstants.TestOutputDirectory);
                if (!Directory.Exists(resultsDirectory))
                {
                    _ = Directory.CreateDirectory(resultsDirectory);
                }

                var status = ComparissonStatus.NewFile;
                var fileName = GenerateResultName(apiCallResult.Item, _config.ResultFileNamePattern);
                var fileOperations = new FileOperations();

                var filePath = Path.Combine(resultsDirectory, fileName);
                var apiResult = JsonSerializer.Serialize(apiCallResult);

                if (fileOperations.ValidateIfFileExists(filePath))
                {
                    var fileSourceResult = JsonSerializer.Deserialize<ApiCallResult>(FileOperations.GetFileData(filePath));

                    if (fileSourceResult is not null)
                    {
                        status = DataComparrison.CompareAPiResults(apiCallResult, fileSourceResult);
                    }
                }
                else
                {
                    await fileOperations.WriteFile(filePath, apiResult);
                }
                return status;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                Errors.Add("Failed to capture logs into a file");
                throw;
            }
        }



        public static string GenerateResultName(DataQueryResult? item, string? resultFileNamePattern = null)
        {
			ArgumentNullException.ThrowIfNull(item);

			var filePrefix = "request";
            var fileSuffix = ".json";

            if (string.IsNullOrWhiteSpace(resultFileNamePattern))
            {
                return $"{filePrefix}-{item?.RowId}{fileSuffix}";
            }
            else
            {
                //we have value lets replace it
                foreach (var resultItem in item.Results)
                {
                    resultFileNamePattern = resultFileNamePattern.Replace($"{{{resultItem.Key}}}", resultItem.Value);
                }

                return $"{filePrefix}-{resultFileNamePattern}{fileSuffix}";
            }
        }

        internal async Task<TestRunner> PrintResultsSummary() {
            Console.WriteLine("==========Status==========");
            foreach (TestResultStatus item in _resultsStats) {
                Console.WriteLine($"{item.StatusCode} - Count: {item.NumberOfResults}");
            }

            if (_errors.Count > 0) {
                Console.WriteLine("==========Errors==========");
                foreach (string error in _errors) {
                    Console.WriteLine(error);
                }
            }
            
            await Task.CompletedTask;
            return this;
        }
		
        //private async Task MakeApiCorCollectionCall(HttpClient client, DataQueryResult item)
        //{
        //    _logger.LogInformation($"API base url: {client.BaseAddress}");
        //    //string? url = string.Empty;
        //    //try
        //    //{
        //    //    url = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, item);
        //    //    if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
        //    //    {
        //    //        compareUrl = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.CompareUrlPath, _config, item);
        //    //    }
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    _errors.Add($"Error has occurred while composing an url: {url}");
        //    //    return;
        //    //}

        //    //await MakeApiForCollectionCall(client, url, item);


        //    return;
        //}
    }
}