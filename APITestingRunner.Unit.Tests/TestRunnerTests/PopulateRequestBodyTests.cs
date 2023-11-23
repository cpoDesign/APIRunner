using APITestingRunner.ApiRequest;
using APITestingRunner.Configuration;
using APITestingRunner.IoOperations;
using FluentAssertions;

namespace APITestingRunner.Unit.Tests
{
    [TestClass]
    public class PopulateRequestBodyTests
    {
        [TestMethod]
        public void PopulateRequestBody_ShouldThrowExcpetion_becauseOfconfig()
        {
            Action action = () => TestRunner.PopulateRequestBody(null, new Database.DataQueryResult()
            {
                RowId = 1,
                Results = [
                    new KeyValuePair<string, string>("requestBody","JoeDoe"),
                    new KeyValuePair<string, string>("bindingId", "3") 
                ]
            });

            _ = action.Should().Throw<ArgumentException>().WithMessage("Value cannot be null. (Parameter 'config')");
        }

        [TestMethod]
        public void PopulateRequestBody_ShouldThrowExcpetion_becauseOfNoDataQueryResult()
        {
            Config config = new()
            {
                UrlBase = "http://localhost:7055",
                CompareUrlBase = string.Empty,
                CompareUrlPath = string.Empty,
                UrlPath = "/WeatherForecast",
                RequestBody = null,
                HeaderParam = [],
                UrlParam = [],
                DBConnectionString = null,
                DBQuery = "select id as bindingId, userName as requestBody from dbo.sampleTable where id in (1,3)",
                DBFields = null,
                RequestType = RequestType.GET,
                ResultsStoreOption = StoreResultsOption.All,
                ResultFileNamePattern = "{fileRecordType}-{bindingId}",
                ConfigMode = TesterConfigMode.CaptureAndCompare,
                OutputLocation = DirectoryServices.AssemblyDirectory,
            };

            Action action = () => TestRunner.PopulateRequestBody(config, null);

            _ = action.Should().Throw<ArgumentException>().WithMessage("Value cannot be null. (Parameter 'dataQueryResult')");
        }

        [TestMethod]
        [DataRow("just", "just")]
        [DataRow(null, "")]
        [DataRow("{\"name\":\"{requestBody}\"}", "{\"name\":\"JoeDoe\"}")]
        [DataRow("{\"name\":\"{requestBody}\",\"id\":\"{bindingId}\"}", "{\"name\":\"JoeDoe\",\"id\":\"3\"}")]
        public void PopulateRequestBody(string request, string expected)
        {

            Config config = new()
            {
                UrlBase = "http://localhost:7055",
                CompareUrlBase = string.Empty,
                CompareUrlPath = string.Empty,
                UrlPath = "/WeatherForecast",
                RequestBody = request,
                HeaderParam = [
                    new Param("accept", "application/json")
                ],
                UrlParam = [
                    new Param("urlKey", "configKey"),
                    new Param("id", "bindingId")
                ],
                DBConnectionString = null,
                DBQuery = "select id as bindingId, userName as requestBody from dbo.sampleTable where id in (1,3)",
                DBFields = [
                    new Param("bindingId", "bindingId"),
                    new Param("requestBody", "requestBody"),
                ],
                RequestType = RequestType.GET,
                ResultsStoreOption = StoreResultsOption.All,
                ResultFileNamePattern = "{fileRecordType}-{bindingId}",
                ConfigMode = TesterConfigMode.CaptureAndCompare,
                OutputLocation = DirectoryServices.AssemblyDirectory,
            };

            var actual = TestRunner.PopulateRequestBody(config, new Database.DataQueryResult()
            {
                RowId = 1,
                Results = [
                    new KeyValuePair<string, string>("requestBody","JoeDoe"),
                    new KeyValuePair<string, string>("bindingId", "3") 
                ]
            });

            Assert.AreEqual(expected, actual);
        }
    }
}