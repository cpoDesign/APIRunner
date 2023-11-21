using FluentAssertions;
using Microsoft.Extensions.Logging;
using WireMock.Matchers.Request;
using WireMock.ResponseBuilders;
using WireMock.Server;
using static ConfigurationManager;

namespace APITestingRunner.Unit.Tests
{

    [TestClass]
    public class TestDifferentRequestTypes : TestBase
    {
        private WireMockServer? server;

        [TestInitialize]
        public new void Initialize()
        {
            base.Initialize();

            // This starts a new mock server instance listening at port 9876
            server = WireMockServer.Start(7055);

            // ref for CORS: https://github.com/WireMock-Net/WireMock.Net/wiki/Cors
            // ref for matching: https://github.com/WireMock-Net/WireMock.Net/wiki/Request-Matching
        }

        [TestCleanup]
        public void Cleanup()
        {

            var expectedFilePath = DirectoryServices.AssemblyDirectory;

            var testDirectory = Path.Combine(expectedFilePath, TestConstants.TestOutputDirectory);
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }

            // This stops the mock server to clean up after ourselves
            _ = server ?? throw new ArgumentNullException(nameof(server));
            server.Stop();
        }

        [TestMethod]
        [TestCategory("SimpleAPICallBasedOnConfig")]
        [DataRow(RequestType.GET)]
        [DataRow(RequestType.POST)]
        [DataRow(RequestType.PUT)]
        [DataRow(RequestType.PATCH)]
        [DataRow(RequestType.DELETE)]
        public async Task RequestDifferentRequestTypes(RequestType request)
        {
            _ = server ?? throw new ArgumentNullException(nameof(server));

            server.Given(SetupMockForARequestType(request))
                .RespondWith(
                    Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "text/plain")
                    .WithBody("Hello, world!")
                );

            Config config = new()
            {
                UrlBase = "http://localhost:7055",
                CompareUrlBase = string.Empty,
                CompareUrlPath = string.Empty,
                UrlPath = "/WeatherForecast",
                UrlParam = null!,
                RequestBody = null,
                HeaderParam = new List<Param> { new Param("accept","application/json") },
                DBConnectionString = null,
                DBQuery = null,
                DBFields = null,
                RequestType = request,
                ResultsStoreOption = StoreResultsOption.None,
                ConfigMode = TesterConfigMode.Run,
                OutputLocation = DirectoryServices.AssemblyDirectory,
            };

            var logger = new TestLogger();
            var testRunner = await new ApiTesterRunner(logger).RunTests(config);

            _ = testRunner.Errors.Should().BeEmpty();
            _ = logger.Messages.Should().ContainEquivalentOf(new Tuple<LogLevel, string>(LogLevel.Information, $"{request} /WeatherForecast 200 success"));
        }

        // TODO: Fix this text wiremock does not recognise the request when validating against a body
        //[TestMethod]
        //[TestCategory("SimpleAPICallBasedOnConfig")]
        //[DataRow(RequestType.POST)]
        //public async Task Request_Post_With_Static_BodyFromConfig(RequestType requestType) {
        //    _ = server ?? throw new ArgumentNullException(nameof(server));
        //    var data = JsonConvert.SerializeObject(
        //        new {
        //            Name = "Test Name",
        //            Age = 5
        //        });
        //    //var dummDataToPost = new StringContent(data, Encoding.UTF8, MediaTypeNames.Application.Json);
        //    server.Given(
        //         WireMock.RequestBuilders.Request.Create().WithPath("/WeatherForecast")
        //         .WithBody("Test")
        //         .UsingPost()
        //   )
        //   .RespondWith(
        //       Response.Create()
        //           .WithStatusCode(200)
        //           .WithHeader("Content-Type", "application/json")
        //           .WithBody(@"{ ""msg"": ""Hello I'm a little bit slow!"" }")
        //   );


        //    Config config = new() {
        //        UrlBase = "http://localhost:7055",
        //        CompareUrlBase = string.Empty,
        //        CompareUrlPath = string.Empty,
        //        UrlPath = "/WeatherForecast",
        //        UrlParam = null,
        //        RequestBody = data, //"{\"Name\":\"Test Name\",\"Age\":5}",
        //        HeaderParam = new List<Param> {
        //                        new Param("accept","application/json")
        //                      },
        //        DBConnectionString = null,
        //        DBQuery = null,
        //        DBFields = null,
        //        RequestType = requestType,
        //        ResultsStoreOption = StoreResultsOption.None,
        //        ConfigMode = TesterConfigMode.Run,
        //        OutputLocation = DirectoryServices.AssemblyDirectory,
        //    };

        //    var logger = new TestLogger();

        //    var testRunner = await new ApiTesterRunner()
        //                        .AddLogger(logger)
        //                        .RunTests(config);

        //    _ = testRunner.Errors.Should().BeEmpty();

        //    _ = logger.Messages.Should().ContainEquivalentOf(new Tuple<LogLevel, string>(LogLevel.Information, $"{requestType} /WeatherForecast 200 success"));
        //}

        private IRequestMatcher SetupMockForARequestType(RequestType request)
        {
            switch (request)
            {
                case RequestType.GET:
                    return WireMock.RequestBuilders.Request.Create().WithPath("/WeatherForecast").UsingGet();
                case RequestType.POST:
                    return WireMock.RequestBuilders.Request.Create().WithPath("/WeatherForecast").UsingPost();
                case RequestType.PUT:
                    return WireMock.RequestBuilders.Request.Create().WithPath("/WeatherForecast").UsingPut();
                case RequestType.PATCH:
                    return WireMock.RequestBuilders.Request.Create().WithPath("/WeatherForecast").UsingPatch();
                case RequestType.DELETE:
                    return WireMock.RequestBuilders.Request.Create().WithPath("/WeatherForecast").UsingDelete();

                default:
                    throw new NotImplementedException();
            }
        }
    }
}