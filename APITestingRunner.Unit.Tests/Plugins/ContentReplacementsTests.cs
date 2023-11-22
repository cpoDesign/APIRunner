using APITestingRunner.Configuration;
using APITestingRunner.Plugins;
using Microsoft.Extensions.Logging;
using Moq;

namespace APITestingRunner.Unit.Tests.Plugins
{
    [TestClass]
    public class ContentReplacementsTests
    {

        [TestMethod]
        [TestCategory("Plugins")]
        [TestCategory("ContentReplacements")]
        public void Test_No_Definition_ShouldReturnSameString()
        {
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

            var plugin = new ContentReplacements();
            plugin.ApplyConfig(ref baseConfig, new Mock<ILogger>().Object);
            var result = plugin.ProcessBeforeSave("test");

            Assert.AreEqual("test", result);
        }

        [TestMethod]
        [TestCategory("Plugins")]
        [TestCategory("ContentReplacements")]
        [DataRow("Test", "Demo")]
        [DataRow("Testing", "Demoing")]
        public void Test_ForOneReplacement(string beforeProcessing, string afterProcessing)
        {
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
                ContentReplacements = new ContentReplacement[] {
                    new ContentReplacement
                    {
                        From = "Test",
                        To = "Demo",
                        StoreInFile = true,
                    }
                }
            };

            var plugin = new ContentReplacements();
            plugin.ApplyConfig(ref baseConfig, new Mock<ILogger>().Object);
            var result = plugin.ProcessBeforeSave(beforeProcessing);

            Assert.AreEqual(afterProcessing, result);
        }

        [TestMethod]
        [TestCategory("Plugins")]
        [TestCategory("ContentReplacements")]
        [DataRow("Test Cat jumping", "Demo Dog jumping")]
        public void Test_ForMultipleReplacement(string beforeProcessing, string afterProcessing)
        {
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
                ContentReplacements = new ContentReplacement[] {
                    new ContentReplacement
                    {
                        From = "Test",
                        To = "Demo",
                        StoreInFile = true,
                    },
                    new ContentReplacement
                    {
                        From = "Cat",
                        To = "Dog",
                        StoreInFile = true,
                    }
                }
            };

            var plugin = new ContentReplacements();
            plugin.ApplyConfig(ref baseConfig, new Mock<ILogger>().Object);
            var result = plugin.ProcessBeforeSave(beforeProcessing);

            Assert.AreEqual(afterProcessing, result);
        }

        [TestMethod]
        [TestCategory("Plugins")]
        [TestCategory("ContentReplacements")]
        [DataRow("TestCat jumping", "Dog jumping")]
        public void Test_ForMultiple_OverridingValuesReplacement(string beforeProcessing, string afterProcessing)
        {
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
                ContentReplacements = new ContentReplacement[] {
                    new ContentReplacement
                    {
                        From = "TestCat",
                        To = "Demo",
                        StoreInFile = true,
                    },
                    new ContentReplacement
                    {
                        From = "Demo",
                        To = "Dog",
                        StoreInFile = true,
                    }
                }
            };

            var plugin = new ContentReplacements();
            plugin.ApplyConfig(ref baseConfig, new Mock<ILogger>().Object);
            var result = plugin.ProcessBeforeSave(beforeProcessing);

            Assert.AreEqual(afterProcessing, result);
        }

        [TestMethod]
        [TestCategory("Plugins")]
        [TestCategory("ContentReplacements")]
        [DataRow("TestCat jumping", "Dog jumping")]
        public void Test_ForMultiple_IgnoresStoreInFileIsFalse(string beforeProcessing, string afterProcessing)
        {
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
                ContentReplacements = new ContentReplacement[] {
                    new ContentReplacement
                    {
                        From = "TestCat",
                        To = "Demo",
                        StoreInFile = true,
                    },
                    new ContentReplacement
                    {
                        From = "Demo",
                        To = "Dog",
                        StoreInFile = true,
                    },
                    new ContentReplacement
                    {
                        From = "Dog jumping",
                        To = "failing test",
                        StoreInFile = false,
                    }
                }
            };

            var plugin = new ContentReplacements();
            plugin.ApplyConfig(ref baseConfig, new Mock<ILogger>().Object);
            var result = plugin.ProcessBeforeSave(beforeProcessing);

            Assert.AreEqual(afterProcessing, result);
        }

        [TestMethod]
        [TestCategory("Plugins")]
        [TestCategory("ContentReplacements")]
        [DataRow("TestCat jumping", "fullReplace")]
        public void Test_ForMultiple_ShouldAppplyAllConfigIrrelevantToSaveConfig(string beforeProcessing, string afterProcessing)
        {
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
                ContentReplacements = new ContentReplacement[] {
                    new ContentReplacement
                    {
                        From = "TestCat",
                        To = "Demo",
                        StoreInFile = true,
                    },
                    new ContentReplacement
                    {
                        From = "Demo",
                        To = "Dog",
                        StoreInFile = true,
                    },
                    new ContentReplacement
                    {
                        From = "Dog jumping",
                        To = "fullReplace",
                        StoreInFile = false,
                    }
                }
            };

            var plugin = new ContentReplacements();
            plugin.ApplyConfig(ref baseConfig, new Mock<ILogger>().Object);
            var result = plugin.ProcessValidation(beforeProcessing);

            Assert.AreEqual(afterProcessing, result);
        }
    }
}
