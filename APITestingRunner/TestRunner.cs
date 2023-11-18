// See https://aka.ms/new-console-template for more information

using APITestingRunner.ApiRequest;
using APITestingRunner.Database;
using APITestingRunner.IoOperations;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using static ConfigurationManager;

namespace APITestingRunner {
    public class TestRunner {
        private Config? _config;
        private readonly IEnumerable<DataQueryResult>? _dbBasedItems = new List<DataQueryResult>();
        private readonly List<string> _errors = new();
        private readonly List<string> responses = new();
        private readonly List<TestResultStatus> _resultsStats = new();
        //private readonly HttpClient? compareClient = null;
        //private readonly string? compareUrl;
        private readonly ILogger _logger;

        public List<string> Errors => _errors;

        /// <summary>
        /// Instance of test runner executing the tests.
        /// </summary>
        /// <param name="logger">Instance of a logger.</param>
        /// <exception cref="ArgumentNullException">Can throw when logger is not provided.</exception>
        public TestRunner(ILogger logger) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        internal async Task ApplyConfig(Config? configSettings) {
            if (configSettings == null) {
                _errors.Add("Failed to load configuration");
            } else {
                _config = configSettings;
            }

            await Task.CompletedTask;
        }

        internal async Task<TestRunner> RunTestsAsync() {
            _ = _config ?? throw new ArgumentNullException(nameof(_config));

            // create a request to the api
            var handler = new HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, errors) => {
                                return true;
                            };

            //TODO: conver to HTTPFactory to produce api calls
            HttpClient client = new(handler) {
                BaseAddress = new Uri(_config.UrlBase)
            };

            //if (!string.IsNullOrWhiteSpace(_config.CompareUrlBase))
            //{
            //    compareClient = new()
            //    {
            //        BaseAddress = new Uri(_config.UrlBase)
            //    };
            //}

            PopulateClientHeadersFromConfig(client, _config.HeaderParam);

            await MakeApiCall(client);

            return this;
        }

        private void PopulateClientHeadersFromConfig(HttpClient client, List<Param> headerParam) {
            _ = _config ?? throw new ArgumentNullException(nameof(_config));

            if (headerParam != null && headerParam.Count > 0) {
                foreach (ConfigurationManager.Param item in _config.HeaderParam) {
                    client.DefaultRequestHeaders.Add(item.Name, item.value);
                }
            }
        }

        private async Task MakeApiCall(HttpClient client) {

            //// execute db data load only has some data in it
            //if (!string.IsNullOrWhiteSpace(config.DBConnectionString) && !string.IsNullOrWhiteSpace(config.DBQuery) && config.DBFields.Count() > 0) {
            //    testRunner = await testRunner.GetDataForTestRunnerDataSet();
            //}

            foreach (DataQueryResult dataQueryResult in await GetDataToProcessAsync()) {

                await MakeApiForCollectionCall(client, dataQueryResult);
            }

            return;
        }

        public async Task<IEnumerable<DataQueryResult>> GetDataToProcessAsync() {
            _ = _config ?? throw new ArgumentNullException(nameof(_config));

            /// return data source
            if (!string.IsNullOrWhiteSpace(_config.DBConnectionString) && !string.IsNullOrWhiteSpace(_config.DBQuery) && _config?.DBFields?.Count() > 0) {
                //TODO: convert to yield
                return await new DataAccess(_config).FetchDataForRunnerAsync();
                //yield return await db.FetchDataForRunnerAsync();
            } else {
                return new List<DataQueryResult> { new DataQueryResult() { RowId = 0 } };
            }
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
        private async Task MakeApiForCollectionCall(HttpClient client, DataQueryResult? item = null, string requestBody = "") {
            _ = _config ?? throw new ArgumentNullException(nameof(_config));

            HttpResponseMessage? response = null;
            string onScreenMessage = string.Empty;

            var pathAndQuery = string.Empty;
            try {
                pathAndQuery = new DataRequestConstructor()
                                    .AddBaseUrl(_config.UrlBase)
                                    .ComposeUrlAddressForRequest(_config.UrlPath, _config, item)
                                    .GetPathAndQuery();

                //if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
                //{
                //    compareUrl = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.CompareUrlPath, _config, null);
                //}
            } catch (Exception) {
                _errors.Add($"Error has occurred while composing an url: {pathAndQuery}");
                return;
            }

            if (string.IsNullOrWhiteSpace(pathAndQuery)) {
                _errors.Add("Failed to compose path and query for API request");
                return;
            }

            try {

                switch (_config.RequestType) {
                    case ConfigurationManager.RequestType.GET:

                        if (string.IsNullOrWhiteSpace(requestBody)) {
                            // we are using only data in url query
                            response = await client.GetAsync(pathAndQuery);
                        } else {
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
                    case ConfigurationManager.RequestType.POST:
                        response = await client.PostAsync(pathAndQuery, CreateRequestContent(requestBody));
                        break;
                    case ConfigurationManager.RequestType.PUT:
                        response = await client.PutAsync(pathAndQuery, CreateRequestContent(requestBody));
                        break;
                    case ConfigurationManager.RequestType.PATCH:
                        response = await client.PatchAsync(pathAndQuery, CreateRequestContent(requestBody));
                        break;
                    case ConfigurationManager.RequestType.DELETE:
                        response = await client.DeleteAsync(pathAndQuery);
                        break;
                    default:
                        _errors.Add("Unsupported request type");
                        break;
                }

                if (response != null) {
                    _resultsStats.Add(new TestResultStatus() {
                        NumberOfResults = 1,
                        StatusCode = (int)response.StatusCode
                    });

                    onScreenMessage = GenerateResponseMessage(_config.RequestType, pathAndQuery, response);
                    string content = await response.Content.ReadAsStringAsync();
                    var fileName = string.Empty;
                    var responseHeaders = response.Headers.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString() ?? string.Empty)).ToList();
                    ProcessingFileResult result = null;
                    switch (_config.ConfigMode) {
                        case TesterConfigMode.Run:
                            // no another work necessary here
                            break;

                        case TesterConfigMode.Capture:
                            fileName = TestRunner.GenerateResultName(item, _config.ResultFileNamePattern);

                            result = await ProcessResultCaptureAndCompareIfRequested(
                                new ApiCallResult(response.StatusCode, content, responseHeaders, pathAndQuery, item, response.IsSuccessStatusCode));
                            if (result.DisplayFilePathInLog) {
                                onScreenMessage += $" {TestConstants.TestOutputDirectory}/{fileName}";
                            }
                            break;

                        case TesterConfigMode.CaptureAndCompare:
                            fileName = TestRunner.GenerateResultName(item, _config.ResultFileNamePattern);


                            result = await ProcessResultCaptureAndCompareIfRequested(new ApiCallResult(response.StatusCode, content, responseHeaders, pathAndQuery, item, response.IsSuccessStatusCode));

                            if (result.DisplayFilePathInLog) {
                                onScreenMessage += $" {TestConstants.TestOutputDirectory}/{fileName}";
                            }
                            onScreenMessage += $" {Enum.GetName(result.ComparissonStatus)}";

                            break;

                        default:
                            _errors.Add("This option is currently not supported");
                            throw new NotImplementedException();
                    }

                    //TODO:implement
                    ///case TesterConfigMode.APICompare:
                    //if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
                    //{
                    //    List<string> compareList = new();

                    //    if (compareClient != null)
                    //    {
                    //        HttpResponseMessage compareResponse = await compareClient.GetAsync(compareUrl);

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
            } catch (Exception ex) {
                _errors.Add($"Error has occurred while calling api with url:{client.BaseAddress} with {pathAndQuery} with message: {ex.Message}");
            } finally {
                _logger.LogInformation(onScreenMessage);
            }
        }

        public string GenerateResponseMessage(RequestType requestType, string relativeUrl, HttpResponseMessage? response) {
            var determination = "fail";

            if (response is not null) {
                if (response.IsSuccessStatusCode) determination = "success";
                return $"{requestType} {relativeUrl} {(int)response.StatusCode} {determination}";
            }

            return $"{relativeUrl} failed to return";
        }

        private static StringContent CreateRequestContent(string requestBody) {
            return new(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);
        }

        private async Task<ProcessingFileResult> ProcessResultCaptureAndCompareIfRequested(ApiCallResult apiCallResult) {
            _ = _config ?? throw new ArgumentNullException(nameof(_config));

            _ = $"{apiCallResult.statusCode} - {apiCallResult.responseContent}";

            ComparissonStatus fileCompareStatus = ComparissonStatus.NewFile;
            var result = new ProcessingFileResult { ComparissonStatus = fileCompareStatus };

            if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.Capture || _config.ConfigMode == ConfigurationManager.TesterConfigMode.CaptureAndCompare) {
                if (_config.ResultsStoreOption == ConfigurationManager.StoreResultsOption.All || (_config.ResultsStoreOption == ConfigurationManager.StoreResultsOption.FailuresOnly && !apiCallResult.IsSuccessStatusCode)) {
                    if (_config.OutputLocation != null) {
                        result.DisplayFilePathInLog = true;
                        result.ComparissonStatus = await logIntoFileAsync(_config.OutputLocation, apiCallResult, false);
                    } else {
                        _errors.Add("No logLocation found");
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
        private async Task<ComparissonStatus> logIntoFileAsync(string logLocation, ApiCallResult apiCallResult, bool validateIfFilesMatch = false) {
            _ = _config ?? throw new ArgumentNullException(nameof(_config));

            string resultsDirectory = Path.Combine(logLocation, TestConstants.TestOutputDirectory);
            if (!Directory.Exists(resultsDirectory)) {
                _ = Directory.CreateDirectory(resultsDirectory);
            }

            var status = ComparissonStatus.NewFile;
            var fileName = TestRunner.GenerateResultName(apiCallResult.item, _config.ResultFileNamePattern);
            var fileOperations = new FileOperations();

            var filePath = Path.Combine(resultsDirectory, fileName);
            string apiResult = JsonSerializer.Serialize(apiCallResult);

            if (fileOperations.ValidateIfFileExists(filePath)) {
                var fileSourceResult = JsonSerializer.Deserialize<ApiCallResult>(FileOperations.GetFileData(filePath));

                if (fileSourceResult is not null) {
                    status = DataComparrison.CompareAPiResults(apiCallResult, fileSourceResult);
                }

            } else {
                await fileOperations.WriteFile(filePath, apiResult);
            }

            return status;
        }



        public static string GenerateResultName(DataQueryResult? item, string? resultFileNamePattern = null) {
            if (item == null) throw new ArgumentNullException(nameof(item));

            string filePrefix = "request";
            string fileSuffix = ".json";

            if (string.IsNullOrWhiteSpace(resultFileNamePattern)) {
                return $"{filePrefix}-{item?.RowId}{fileSuffix}";
            } else {

                //we have value lets replace it

                foreach (var resultItem in item.Results) {
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

        private async Task MakeApiCorCollectionCall(HttpClient client, DataQueryResult item) {
            _logger.LogInformation($"API base url: {client.BaseAddress}");
            //string? url = string.Empty;
            //try
            //{
            //    url = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, item);
            //    if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
            //    {
            //        compareUrl = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.CompareUrlPath, _config, item);
            //    }
            //}
            //catch (Exception)
            //{
            //    _errors.Add($"Error has occurred while composing an url: {url}");
            //    return;
            //}

            //await MakeApiForCollectionCall(client, url, item);


            return;
        }
    }
    public class DataComparrison {


        public static ComparissonStatus CompareAPiResults(ApiCallResult apiCallResult, ApiCallResult fileSourceResult) {
            var status = ComparissonStatus.Different;

            if (apiCallResult.statusCode == fileSourceResult.statusCode)
                if (apiCallResult.IsSuccessStatusCode == fileSourceResult.IsSuccessStatusCode)
                    if (apiCallResult.responseContent == fileSourceResult.responseContent)
                        status = ComparissonStatus.Matching;

            return status;
        }
    }
}