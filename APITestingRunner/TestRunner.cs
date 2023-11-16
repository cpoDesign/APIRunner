// See https://aka.ms/new-console-template for more information

using APITestingRunner.ApiRequest;
using APITestingRunner.Database;
using APITestingRunner.IoOperations;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Text;
using static ConfigurationManager;

namespace APITestingRunner {
    public class TestRunner {
        private Config _config;
        private readonly IEnumerable<DataQueryResult>? _dbBasedItems = new List<DataQueryResult>();
        private readonly List<string> _errors = new();
        private readonly List<string> responses = new();
        private readonly List<TestResultStatus> _resultsStats = new();
        private readonly HttpClient? compareClient = null;
        private readonly string? compareUrl;
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
            // create a request to the api
            //TODO: conver to HTTPFactory to produce api calls

            var handler = new HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, errors) => {
                                return true;
                            };

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

            //if (_dbBasedItems.Count() > 0)
            //{
            //    //http://localhost:5152/Data?id=1
            //    foreach (DataQueryResult item in _dbBasedItems)
            //    {
            //        Console.WriteLine($"proceeding with call for record {item.RowId}");

            //        await MakeApiCorCollectionCall(client, item);
            //    }
            //}
            //else
            //{
            //    //http://localhost:5152/Data?id=1
            //    await MakeApiCall(client);
            //}

            await MakeApiCall(client);

            return this;
        }

        private void PopulateClientHeadersFromConfig(HttpClient client, List<Param> headerParam) {
            if (headerParam != null && headerParam.Count > 0) {
                foreach (ConfigurationManager.Param item in _config.HeaderParam) {
                    client.DefaultRequestHeaders.Add(item.Name, item.value);
                }
            }
        }

        private async Task MakeApiCall(HttpClient client) {
            string? pathAndQuery = string.Empty;


            //// execute db data load only has some data in it
            //if (!string.IsNullOrWhiteSpace(config.DBConnectionString) && !string.IsNullOrWhiteSpace(config.DBQuery) && config.DBFields.Count() > 0) {
            //    testRunner = await testRunner.GetDataForTestRunnerDataSet();
            //}


            try {
                pathAndQuery = new DataRequestConstructor().AddBaseUrl(_config.UrlBase).ComposeUrlAddressForRequest(_config.UrlPath, _config, null).GetPathAndQuery();

                //if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.APICompare)
                //{
                //    compareUrl = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.CompareUrlPath, _config, null);
                //}
            } catch (Exception) {
                _errors.Add($"Error has occurred while composing an url: {pathAndQuery}");
                return;
            }

            foreach (DataQueryResult dataQueryResult in await GetDataToProcessAsync()) {

                await MakeApiForCollectionCall(client, pathAndQuery, dataQueryResult);
            }

            return;
        }

        public async Task<IEnumerable<DataQueryResult>> GetDataToProcessAsync() {

            /// return data source
            if (!string.IsNullOrWhiteSpace(_config.DBConnectionString) && !string.IsNullOrWhiteSpace(_config.DBQuery) && _config.DBFields.Count() > 0) {
                //TODO: convert to yield
                return await new DataAccess(_config).FetchDataForRunnerAsync();
                //yield return await db.FetchDataForRunnerAsync();
            } else {
                return new List<DataQueryResult> { new DataQueryResult() { RowId = 0 } };
            }
        }


        //internal async Task<TestRunner> GetDataForTestRunnerDataSet() {
        //    // connect to database and load information
        //    DataAccess db = new(_config);
        //    _dbBasedItems = await db.FetchDataForRunnerAsync();

        //    if (_dbBasedItems != null) {
        //        Console.WriteLine($"Found {_dbBasedItems.Count()} records for test");
        //    }

        //    return this;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client">Client contains base url</param>
        /// <param name="pathAndQuery">contains the relative paths</param>
        /// <param name="item"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task MakeApiForCollectionCall(HttpClient client, string pathAndQuery, DataQueryResult? item = null, string requestBody = "") {
            HttpResponseMessage? response = null;
            string onScreenMessage = string.Empty;

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
                    //case ConfigurationManager.RequestType.POST:
                    //    response = await client.PostAsync(pathAndQuery, CreateRequestContent(requestBody));
                    //    break;
                    //case ConfigurationManager.RequestType.PUT:
                    //    response = await client.PutAsync(pathAndQuery, CreateRequestContent(requestBody));
                    //    break;
                    //case ConfigurationManager.RequestType.PATCH:
                    //    response = await client.PatchAsync(pathAndQuery, CreateRequestContent(requestBody));
                    //    break;
                    //case ConfigurationManager.RequestType.DELETE:
                    //    response = await client.DeleteAsync(pathAndQuery);
                    //    break;
                    default:
                        _errors.Add("Unsupported request type");
                        break;
                }

                if (response != null) {
                    _resultsStats.Add(new TestResultStatus() {
                        NumberOfResults = 1,
                        StatusCode = (int)response.StatusCode
                    });


                    onScreenMessage = GenerateResponseMessage(pathAndQuery, response);

                    string content = await response.Content.ReadAsStringAsync();


                    switch (_config.ConfigMode) {
                        case TesterConfigMode.Run:
                            // no another work necessary here
                            break;
                        case TesterConfigMode.Capture:

                            string fileName = TestRunner.GenerateResultName(item);

                            onScreenMessage += $" {TestConstants.TestOutputDirectory}/{fileName}";
                            await ProcessResultCapture(
                                new ApiCallResult(response.StatusCode, content, response.Headers, pathAndQuery, item, response.IsSuccessStatusCode));

                            break;
                        case TesterConfigMode.FileCaptureAndCompare:

                            throw new NotImplementedException();
                            //await ProcessResultCapture(new ApiCallResult(response.StatusCode, content, response.Headers, pathAndQuery, item, response.IsSuccessStatusCode), true);

                            break;
                        case TesterConfigMode.APICompare:

                        default:
                            _errors.Add("This option is currently not supported");
                            throw new NotImplementedException();
                    }



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

        public string GenerateResponseMessage(string relativeUrl, HttpResponseMessage? response) {
            var determination = "fail";
            if (response != null && response.IsSuccessStatusCode) {
                determination = "success";
            }

            return $"{relativeUrl} {(int)response.StatusCode} {determination}";
        }

        private static StringContent CreateRequestContent(string requestBody) {
            return new(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json);
        }

        private async Task ProcessResultCapture(ApiCallResult apiCallResult, bool IsCompareFile = false) {
            string response = $"{apiCallResult.statusCode} - {apiCallResult.responseContent}";

            if (_config.ConfigMode == ConfigurationManager.TesterConfigMode.Capture) {
                if (_config.ResultsStoreOption == ConfigurationManager.StoreResultsOption.All || (_config.ResultsStoreOption == ConfigurationManager.StoreResultsOption.FailuresOnly && !apiCallResult.IsSuccessStatusCode)) {
                    if (_config.OutputLocation != null) {
                        await logIntoFileAsync(_config.OutputLocation, apiCallResult, IsCompareFile);
                    } else {
                        _errors.Add("No logLocation found");
                    }
                }
            }

            if (IsCompareFile) {
                TestResultStatus? existingResult = _resultsStats.FirstOrDefault(x => x.StatusCode == (int)apiCallResult.statusCode);
                if (existingResult == null) {
                    _resultsStats.Add(new TestResultStatus { StatusCode = (int)apiCallResult.statusCode, NumberOfResults = 1 });
                } else {
                    existingResult.NumberOfResults++;
                }

                responses.Add(response);
            }

            Console.WriteLine(response);
        }

        private async Task logIntoFileAsync(string logLocation, ApiCallResult apiCallResult, bool IsCompareFile) {
            string resultsDirectory = Path.Combine(logLocation, TestConstants.TestOutputDirectory);
            if (!Directory.Exists(resultsDirectory)) {
                _ = Directory.CreateDirectory(resultsDirectory);
            }

            var fileName = TestRunner.GenerateResultName(apiCallResult.item);

            if (IsCompareFile) {
                fileName += "Compare";
            }

            await new FileOperations().WriteFile(resultsDirectory, fileName, apiCallResult);

        }

        public static string GenerateResultName(DataQueryResult item) {
            if (item == null) throw new ArgumentNullException(nameof(item));

            string filePrefix = "request";
            return $"{filePrefix}-{item?.RowId}.json";
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
}