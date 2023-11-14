using APITestingRunner.Unit.Tests.SampleDatabase;
using static ConfigurationManager;

namespace APITestingRunner.Unit.Tests
{
    [TestClass]
    public class TestRunnerWithOptionsTests
    {
        [TestMethod()]
        public async Task ValidateImplementationForSingleAPICallAsync()
        {

            Config config = new()
            {
                UrlBase = "https://localhost:7055/WeatherForecast",
                CompareUrlBase = string.Empty,
                CompareUrlPath = string.Empty,
                UrlPath = "/WeatherForecast",
                UrlParam = null,
                RequestBody = null,

                HeaderParam = new List<Param> {
        new Param("accept","application/json")
      },
                DBConnectionString = null,
                DBQuery = null,
                DBFields = null,
                RequestType = RequestType.GET,
                ResultsStoreOption = StoreResultsOption.None,
                ConfigMode = TesterConfigMode.Run,
                LogLocation = DirectoryServices.AssemblyDirectory
            };

            await IndividualActions.RunTests(config);
        }

        [TestMethod]
        public async Task CreateConfigForSingleAPICall()
        {
            Config config = new()
            {
                UrlBase = "https://localhost:7055/WeatherForecast",
                CompareUrlBase = string.Empty,
                CompareUrlPath = string.Empty,
                UrlPath = "/WeatherForecast",
                UrlParam = null,
                RequestBody = null,

                HeaderParam = new List<Param> {
          new Param("accept","application/json")
        },
                DBConnectionString = null,
                DBQuery = null,
                DBFields = null,
                RequestType = RequestType.GET,
                ResultsStoreOption = StoreResultsOption.None,
                ConfigMode = TesterConfigMode.Run,
                LogLocation = DirectoryServices.AssemblyDirectory
            };

            await IndividualActions.RunTests(config);
        }

        [TestMethod]
        public async Task CreateConfigForSingleAPICallWithUrlParam()
        {
            Config config = new()
            {
                UrlBase = "https://localhost:7055",
                CompareUrlBase = string.Empty,
                CompareUrlPath = string.Empty,
                UrlPath = "/WeatherForecast/GetWeatherForecastForLocation",
                UrlParam = new List<Param>
          {
            new Param("location","UK")
          },
                HeaderParam = new List<Param> {
          new Param("accept","application/json")
        },
                RequestBody = null,
                DBConnectionString = null,
                DBQuery = null,
                DBFields = null,
                RequestType = RequestType.GET,
                ResultsStoreOption = StoreResultsOption.None,
                ConfigMode = TesterConfigMode.Run,
                LogLocation = DirectoryServices.AssemblyDirectory
            };
        }

        //[DataRow(StoreResultsOption.None)]
        //[DataRow(StoreResultsOption.FailuresOnly)]
        //[DataRow(StoreResultsOption.All)]
        //public async Task CreateConfigForDatabaseBasedAPICall(StoreResultsOption storeResultsOption)
        //{
        [TestMethod]
        public async Task CreateConfigForDatabaseBasedAPICall()
        {
            StoreResultsOption storeResultsOption = StoreResultsOption.All;

            string sqlCon = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\code\cpoDesign\APITestingRunner\APITestingRunner.Unit.Tests\SampleDb.mdf;Integrated Security=True";


            Config config = new()
            {
                UrlBase = "https://localhost:7055",
                CompareUrlBase = string.Empty,
                CompareUrlPath = string.Empty,
                UrlPath = "/Data",
                UrlParam = new List<Param>
        {
          new Param("urlKey", "test"),
          new Param("id", "sqlId")
        },
                HeaderParam = new List<Param> {
          new Param("accept","application/json")
        },
                RequestBody = null,
                DBConnectionString = sqlCon,
                DBQuery = "select id as sqlId from dbo.sampleTable;",
                DBFields = new List<Param>
        {
          new Param("sqlId", "sqlId")
        },
                RequestType = RequestType.GET,
                ResultsStoreOption = storeResultsOption,
                ConfigMode = TesterConfigMode.Run,
                LogLocation = DirectoryServices.AssemblyDirectory
            };


            await IndividualActions.RunTests(config);
        }

        //[TestMethod]
        //public async Task CreateConfigForDatabaseBasedAPIComparrisonCall()
        //{
        //    Config config = new()
        //    {
        //        UrlBase = "https://localhost:7055",
        //        CompareUrlBase = "https://localhost:7055",
        //        UrlPath = "/Data",
        //        CompareUrlPath = "/DataV2",
        //        UrlParam = new List<Param>
        //{
        //  new Param("urlKey", "test"),
        //  new Param("id", "sqlId")
        //},
        //        HeaderParam = new List<Param> {
        //  new Param("accept","application/json")
        //},
        //        RequestBody = null,
        //        DBConnectionString = "Server=127.0.0.1; Database=test; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True;",
        //        DBQuery = "select id as sqlId from dbo.sampleTable;",
        //        DBFields = new List<Param>
        //{
        //  new Param("sqlId", "sqlId")
        //},
        //        RequestType = RequestType.GET,
        //        ResultsStoreOption = StoreResultsOption.None,
        //        ConfigMode = TesterConfigMode.APICompare,
        //        LogLocation = DirectoryServices.AssemblyDirectory
        //    };


        //    await IndividualActions.RunTests(config);
        //}

        //[TestMethod]
        //public async Task CreateConfigForSingleAPICallWithUrlParamAndBodyModel()
        //{

        //    Config config = new()
        //    {
        //        UrlBase = "https://localhost:7055",
        //        CompareUrlBase = string.Empty,
        //        CompareUrlPath = string.Empty,
        //        UrlPath = "/datamodel/123456789",
        //        UrlParam = new List<Param>
        //  {
        //    new Param("location","UK")
        //  },
        //        HeaderParam = new List<Param> {
        //  new Param("accept","application/json")
        //},
        //        RequestBody = "{Id={sqlId},StaticData=\"data\"}",
        //        DBConnectionString = null,
        //        DBQuery = null,
        //        DBFields = null,
        //        RequestType = RequestType.GET,
        //        ResultsStoreOption = StoreResultsOption.None,
        //        ConfigMode = TesterConfigMode.Run,
        //        LogLocation = DirectoryServices.AssemblyDirectory
        //    };

        //    await IndividualActions.RunTests(config);
        //}
    }
}