using APITestingRunner.ApiRequest;
using APITestingRunner.Configuration;
using APITestingRunner.Database;
using FluentAssertions;

namespace APITestingRunner.Unit.Tests.DataTests
{
    [TestClass]
    public class DataRequestTests
    {
        private Config? _config;

        [TestInitialize]
        public void TestInit()
        {
            _config = new()
            {
                UrlBase = "http://localhost:5152/",
                CompareUrlBase = null,
                CompareUrlPath = null,
                UrlPath = "/Data",
                UrlParam = [
                    new Param("urlKey", "test"),
                    new Param("id", "sqlId")
                ],
                HeaderParam = [
                    new Param("accept", "application/json")
                ],
                RequestBody = null,
                DBConnectionString = DatabaseConnectionBuilder.BuildLocalhostDatabaseConnection(),
                DBQuery = "select id as sqlId from dbo.sampleTable;",
                DBFields = [
                    new Param("sqlId", "sqlId"),
                ],
                RequestType = RequestType.GET,
                ResultsStoreOption = StoreResultsOption.All,
                ConfigMode = TesterConfigMode.Run,
                OutputLocation = DirectoryServices.AssemblyDirectory
            };
        }

        [TestMethod]
        public void DataRequestGetFullUrl()
        {
            var result = new DataRequestConstructor().AddBaseUrl(_config!.UrlBase).GetFullUrl();
            Assert.AreEqual("http://localhost:5152/", result);
        }


        [TestMethod]
        public void DataRequestGetFullUrlWithEndpoint()
        {
            _config!.UrlParam = [];
            var result = new DataRequestConstructor()
                            .AddBaseUrl("http://localhost:5152")
                            .ComposeUrlAddressForRequest(_config.UrlPath, _config, null)
                            .GetFullUrl();

            _ = result.Should().BeEquivalentTo("http://localhost:5152/Data");
        }

        [TestMethod]
        public void DataRequestGetOnlyPathAndQuery()
        {
            _config!.UrlParam = [];
            var result = new DataRequestConstructor()
                            .AddBaseUrl("http://localhost:5152")
                            .ComposeUrlAddressForRequest(_config.UrlPath, _config, null)
                            .GetPathAndQuery();

            _ = result.Should().BeEquivalentTo("/Data");
        }

        [TestMethod]
        public void ComposeRequest_PassNullUrlParam_WillSkipArgumentGeneration()
        {
            _config!.UrlParam = null!;
            var result = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, null).GetFullUrl();
            Assert.AreEqual("/Data", result);
        }

        [TestMethod]
        public void ComposeRequest_PassNullUrlParam_WillSkipArgumentDbMatching()
        {
            _config!.UrlParam =
                [
                    new Param("location", "Paris")
                ];
            var result = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, null).GetFullUrl();
            Assert.AreEqual("/Data?location=Paris", result);
        }

        [TestMethod]
        public void ComposeRequest_PassEmptyUrlParam_ReturnVanillaPath()
        {
            _config!.UrlParam = [];
            var result = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, null).GetFullUrl();
            Assert.AreEqual("/Data", result);
        }

        [TestMethod]
        public void ComposeRequest_PassNullUrlParam_AppliesArgumentGeneration()
        {
            ArgumentNullException.ThrowIfNull(_config);

            var result = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, null).GetFullUrl();
            Assert.IsTrue(result!.Contains("urlKey"));
            Assert.IsTrue(result.Contains("id"));
            Assert.AreEqual("/Data?urlKey=test&id=sqlId", result);
        }

        [TestMethod]
        public void ComposeRequest_PassNullDBFields_AppliesArgumentGeneration()
        {
            ArgumentNullException.ThrowIfNull(_config);

            _config!.DBFields = null;
            var result = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, null).GetFullUrl();
            Assert.IsTrue(result!.Contains("urlKey"));
            Assert.IsTrue(result.Contains("id"));
            Assert.AreEqual("/Data?urlKey=test&id=sqlId", result);
        }

        [TestMethod]
        public void ComposeRequest_PassMatchingDbFieldReplacesValues()
        {

            var dbResult = new List<KeyValuePair<string, string>> { new("sqlId", "123") };

            var result = new DataRequestConstructor().ComposeUrlAddressForRequest(_config!.UrlPath, _config, new DataQueryResult
            {
                RowId = 1,
                Results = dbResult
            }).GetFullUrl();

            Assert.IsTrue(result!.Contains("urlKey"));
            Assert.IsTrue(result.Contains("id"));
            Assert.AreEqual("/Data?urlKey=test&id=123", result);
        }

        [TestMethod]
        public void ComposeRequest_PassMatchingDbFieldReplacesValuesMultiple()
        {
            ArgumentNullException.ThrowIfNull(_config);

            var dbResult = new List<KeyValuePair<string, string>> {
                new("sqlId", "123"),
                new("sqlName", "joe")
              };

            _config.UrlParam.Add(new Param("name", "sqlName"));

            // add the configuration to ensure the application is aware of the mapping
            _config.DBFields!.Add(new Param("sqlName", "sqlName"));

            var result = new DataRequestConstructor().ComposeUrlAddressForRequest(_config.UrlPath, _config, new DataQueryResult
            {
                RowId = 1,
                Results = dbResult
            })
                .GetFullUrl();

            Assert.IsTrue(result!.Contains("urlKey"));
            Assert.IsTrue(result.Contains("id"));
            Assert.AreEqual("/Data?urlKey=test&id=123&name=joe", result);
        }
    }
}