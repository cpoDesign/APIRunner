using static ConfigurationManager;

namespace APITestingRunner.Unit.Tests
{
  [TestClass]
  public class DataAccessTests
  {
    private Config? _config;

    [TestInitialize]
    public void TestInit()
    {
      _config = new()
      {
        UrlBase = "http://localhost:5152/",
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
        DBConnectionString = "Server=127.0.0.1; Database=test; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True;",
        DBQuery = "select id as sqlId from dbo.sampleTable;",
        DBFields = new List<Param>
      {
        new Param("sqlId", "sqlId")
      },
        RequestType = RequestType.GET,
        ResultsStoreOption = StoreResultsOption.All,
        ConfigMode = TesterConfigMode.Run,
        LogLocation = DirectoryServices.AssemblyDirectory
      };
    }

    [TestMethod]
    public void DataAccess_Tests_ConstructorShouldPass()
    {
      _ = new DataAccess(_config);
    }

    [TestMethod]
    public void DataAccess_Tests_ConstructorShouldThrowArgumentNullException()
    {
      _ = Assert.ThrowsException<ArgumentNullException>(() => new DataAccess(null));
    }

    [TestMethod]
    public async Task FetchDataForRunnerAsync_PassNullForConnectionString_shouldThrowConfigurationErrorsException()
    {
      Config testConfig = _config;
      try
      {
        testConfig.DBConnectionString = null;
        DataAccess da = new(testConfig);
        _ = await da.FetchDataForRunnerAsync();
        Assert.Fail();
      }
      catch (TestRunnerConfigurationErrorsException ex)
      {
        Assert.AreEqual("Failed to load connection string", ex.Message);
      }
      catch
      {
        Assert.Fail();
      }
    }
  }
}