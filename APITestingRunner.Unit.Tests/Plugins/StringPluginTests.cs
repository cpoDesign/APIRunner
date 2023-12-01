using APITestingRunner.ApiRequest;
using APITestingRunner.Configuration;
using APITestingRunner.IoOperations;
using APITestingRunner.Plugins;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITestingRunner.Unit.Tests.Plugins
{
    [TestClass]
    public class StringPluginTests
    {
        private IPlugin? _stringComparisonPlugin = new StringComparisonPlugin();

        private TestLogger _logger = new();

        [TestInitialize]
        public void Initialize()
        {
            _stringComparisonPlugin = new StringComparisonPlugin();
            IConfig baseConfig = new Config()
            {
                UrlBase = "http://localhost:7055",
                CompareUrlBase = string.Empty,
                CompareUrlPath = string.Empty,
                UrlPath = "/WeatherForecast",
                UrlParam = null,
                RequestBody = null,
                HeaderParam = null,
                DBConnectionString = null,
                DBQuery = null,
                DBFields = null,
                RequestType = RequestType.GET,
                ResultsStoreOption = StoreResultsOption.None,
                ConfigMode = TesterConfigMode.Run,
                OutputLocation = DirectoryServices.AssemblyDirectory,
                ResultFileNamePattern = null,
                ContentReplacements = null
            };

            _logger = new TestLogger();
            _stringComparisonPlugin.ApplyConfig(ref baseConfig, _logger);
        }

        [TestMethod]
        public void NoDifferencesReported()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            var json2 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            var apiResult1 = CreateApiResultForJsonResponse(json1);
            var apiResult2 = CreateApiResultForJsonResponse(json2);

            _ = _stringComparisonPlugin.ProcessComparison(apiResult1, apiResult2, ComparisonStatus.NewFile);
            _ = _logger.Messages.Should().HaveCount(1);
            _ = _logger.Messages.First().Item2.Should().Contain("Source and target has same length.");
        }


        [TestMethod]
        public void DifferentLength_isReported()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\",\"country\":\"USA\"}";
            var apiResult1 = CreateApiResultForJsonResponse(json1);
            var apiResult2 = CreateApiResultForJsonResponse(json2);

            _ = _stringComparisonPlugin.ProcessComparison(apiResult1, apiResult2, ComparisonStatus.NewFile);
            var loggerMessages = _logger.Messages.Select(x => x.Item2);
            _ = loggerMessages.Should().HaveCountGreaterThan(1);
            _ = loggerMessages.Should().ContainMatch("Source is different in length: 42 < 58");
        }

        [TestMethod]
        public void DifferentLength_KeyMissingIsReported_FromTarget()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\",\"country\":\"USA\"}";

            var apiResult1 = CreateApiResultForJsonResponse(json1);
            var apiResult2 = CreateApiResultForJsonResponse(json2);

            var loggerMessages = _logger.Messages.Select(x => x.Item2);
            _ = _stringComparisonPlugin.ProcessComparison(apiResult1, apiResult2, ComparisonStatus.NewFile);
            _ = _logger.Messages.Should().HaveCountGreaterThan(1);
            _ = loggerMessages.Should().ContainMatch("Difference at path '.country' for property 'country'");
            _ = loggerMessages.Should().ContainMatch("Missing path in source at path: '.country' for property 'country'");
        }

        [TestMethod]
        public void DifferentLength_KeyMissingIsReported_FromSource()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\", \"country\":\"USA\"}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\"}";

            var apiResult1 = CreateApiResultForJsonResponse(json1);
            var apiResult2 = CreateApiResultForJsonResponse(json2);

            _ = _stringComparisonPlugin.ProcessComparison(apiResult1, apiResult2, ComparisonStatus.NewFile);
            
            var loggerMessages = _logger.Messages.Select(x => x.Item2);

            _ = loggerMessages.Should().HaveCountGreaterThan(1);
            _ = loggerMessages.Should().ContainMatch("Difference at path '.country' for property 'country'");
            _ = loggerMessages.Should().ContainMatch("Missing path in source at path: '.country' for property 'country'");
        }

        [TestMethod]
        public void Different_DifferentValueIsReported()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            var json2 = "{\"name\":\"John\",\"age\":30,\"city\":\"New Yorks\"}";

            var apiResult1 = CreateApiResultForJsonResponse(json1);
            var apiResult2 = CreateApiResultForJsonResponse(json2);

            _ = _stringComparisonPlugin.ProcessComparison(apiResult1, apiResult2, ComparisonStatus.NewFile);
            var loggerMessages = _logger.Messages.Select(x => x.Item2);
            _ = loggerMessages.Should().HaveCount(4);
            _ = loggerMessages.Should().ContainMatch("Difference at path '' for property 'Root'");
            _ = loggerMessages.Should().ContainMatch("Difference at path '.city' for property 'city'");
            _ = loggerMessages.Should().ContainMatch("DiffValue is: New York <> New Yorks");
        }
        [TestMethod]
        public void StringPlugin_ArrayValues()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\",\"hobbies\":[\"reading\",\"traveling\"]}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\",\"hobbies\":[\"reading\",\"cooking\"]}";

            var apiResult1 = CreateApiResultForJsonResponse(json1);
            var apiResult2 = CreateApiResultForJsonResponse(json2);

            _ = _stringComparisonPlugin.ProcessComparison(apiResult1, apiResult2, ComparisonStatus.NewFile);
            var loggerMessages = _logger.Messages.Select(x => x.Item2);
            _ = loggerMessages.Should().HaveCount(5);
            _ = loggerMessages.Should().ContainMatch("Difference at path '.hobbies' for property 'hobbies'");
            _ = loggerMessages.Should().ContainMatch("Difference at path '.hobbies[1]' for property 'hobbies'");
            _ = loggerMessages.Should().ContainMatch("DiffValue is: traveling <> cooking");
        }

        [TestMethod]
        public void Different_DeepArrayNestingComparison()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\",\"hobbies\":[{\"type\":\"reading\",\"locations\":[{\"name\":\"Library\",\"hours\":9},{\"name\":\"Park\",\"hours\":5}]}]}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\",\"hobbies\":[{\"type\":\"reading\",\"locations\":[{\"name\":\"Library\",\"hours\":9},{\"name\":\"Beach\",\"hours\":8}]}]}";

            var apiResult1 = CreateApiResultForJsonResponse(json1);
            var apiResult2 = CreateApiResultForJsonResponse(json2);

            _ = _stringComparisonPlugin.ProcessComparison(apiResult1, apiResult2, ComparisonStatus.NewFile);
            _ = _logger.Messages.Should().HaveCount(9);
            var loggerMessages = _logger.Messages.Select(x => x.Item2);
            _ = loggerMessages.Should().ContainMatch("Difference at path '.hobbies' for property 'hobbies'");
            _ = loggerMessages.Should().ContainMatch("Difference at path '.hobbies[0]' for property 'hobbies'");
            _ = loggerMessages.Should().ContainMatch("Difference at path '.hobbies[0].locations' for property 'locations'");
            _ = loggerMessages.Should().ContainMatch("Difference at path '.hobbies[0].locations[1]' for property 'locations'");
            _ = loggerMessages.Should().ContainMatch("Difference at path '.hobbies[0].locations[1].name' for property 'name'");
            _ = loggerMessages.Should().ContainMatch("DiffValue is: Park <> Beach");
            _ = loggerMessages.Should().ContainMatch("Difference at path '.hobbies[0].locations[1].hours' for property 'hours'");
        }


        private static ApiCallResult CreateApiResultForJsonResponse(string json1)
        {
            return new ApiCallResult
            {
                StatusCode = System.Net.HttpStatusCode.Continue,
                ResponseContent = json1,
                Headers = new List<KeyValuePair<string, string>>(),
                Url = string.Empty,
                DataQueryResult = new Database.DataQueryResult(),
                IsSuccessStatusCode = true,
                CompareResults = new List<string>()
            };
        }
    }
}
