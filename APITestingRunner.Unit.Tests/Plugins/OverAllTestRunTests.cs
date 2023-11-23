namespace APITestingRunner.Unit.Tests.Plugins
{
    [TestClass]
    public class OverAllTestRunTests
    {

        [TestMethod]
        [TestCategory("Plugins")]
        [TestCategory("OverAllTestRun")]
        public void Test_No_Definition_ShouldReturnSameString()
        {
            //todo:
            //IConfig baseConfig = new Config()
            //{
            //    UrlBase = "http://localhost:7055",
            //    CompareUrlBase = string.Empty,
            //    CompareUrlPath = string.Empty,
            //    UrlPath = "/WeatherForecast",
            //    UrlParam = null,
            //    RequestBody = null,
            //    HeaderParam = null,
            //    DBConnectionString = null,
            //    DBQuery = null,
            //    DBFields = null,
            //    RequestType = RequestType.GET,
            //    ResultsStoreOption = StoreResultsOption.None,
            //    ConfigMode = TesterConfigMode.Run,
            //    OutputLocation = DirectoryServices.AssemblyDirectory,
            //    ResultFileNamePattern = null,
            //    ContentReplacements = null
            //};

            //var plugin = new ContentReplacements();
            //plugin.ApplyConfig(ref baseConfig, new Mock<ILogger>().Object);
            //var result = plugin.ProcessBeforeSave("test");

            //Assert.AreEqual("test", result);
        }


    }
}
