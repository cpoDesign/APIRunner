// See https://aka.ms/new-console-template for more information


using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Reflection;

namespace APITestingRunner {
  /// <summary>
  ///     Provides an eval/print loop for command line argument strings.
  ///     TODO: implement command line binder https://learn.microsoft.com/en-us/dotnet/standard/commandline/model-binding
  /// </summary>
  internal static class Program {

    private static bool CreateConfig { get; set; }

    private static bool RunTest { get; set; }

    private static bool Help { get; set; }

    private static readonly Config ApiTesterConfig = new() {
      UrlBase = "http://localhost:7055",
      CompareUrlBase = string.Empty,
      CompareUrlPath = string.Empty,
      UrlPath = "/WeatherForecast",
      RequestBody = null,
      HeaderParam = new List<Param> {
                                new Param("accept","application/json")
                              },
      UrlParam = new List<Param>
                {
                  new Param("urlKey", "configKey"),
                  new Param("id", "bindingId")
                },
      DBConnectionString = null,
      DBQuery = null,
      DBFields = null,
      RequestType = RequestType.GET,
      ResultsStoreOption = StoreResultsOption.None,
      ConfigMode = TesterConfigMode.Run,
      OutputLocation = DirectoryServices.AssemblyDirectory,
    };

    /// <summary>
    ///     Returns a "pretty" string representation of the provided Type; specifically, corrects the naming of generic Types
    ///     and appends the type parameters for the type to the name as it appears in the code editor.
    /// </summary>
    /// <param name="type">The type for which the colloquial name should be created.</param>
    /// <returns>A "pretty" string representation of the provided Type.</returns>
    public static string ToColloquialString(this Type type) {
      return !type.IsGenericType ? type.Name : type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(a => a.ToColloquialString())) + ">";
    }

    /// <summary>
    ///     Application entry point
    /// </summary>
    /// <param name="args">Command line arguments</param>
    private static async Task Main(string[] args) {


      using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
      ILogger logger = loggerFactory.CreateLogger<ApiTesterRunner>();

      //#region sample
      //var delayOption = new Option<int>
      // ("--delay", "An option whose argument is parsed as an int.");

      //var messageOption = new Option<string>
      //    ("--message", "An option whose argument is parsed as a string.");

      //#endregion

      var generateConfig = new Option<bool>
       ("--generateConfig", "An option to generate a new config file with sample data.");

      //TODO: add a option for people to provide a custom config path
      //var config = new Option<string>
      // ("--config", "A path to a custom config file to be used for the runner.");

      var run = new Option<bool>
       ("--run", "Run the tester.");

      var version = new Option<bool>
       ("--version", "Print version of this tool.");

      var rootCommand = new RootCommand("Parameter binding example");
      //rootCommand.Add(delayOption);
      //rootCommand.Add(messageOption);

      //rootCommand.SetHandler(
      //    (delayOptionValue, messageOptionValue) => {
      //      DisplayIntAndString(delayOptionValue, messageOptionValue);
      //    },
      //    delayOption, messageOption);

      //rootCommand.Add(config);
      rootCommand.Add(generateConfig);
      rootCommand.Add(run);


      string pathConfigJson = $"{DirectoryServices.AssemblyDirectory}\\config.json";

      rootCommand.SetHandler((version) => {
        logger.LogInformation(Assembly.GetEntryAssembly().GetName().Version.MajorRevision.ToString());

      }, version);

      //rootCommand.SetHandler(async (generateConfig) => {
      //  logger.LogInformation($"Started a sample config generation.");

      //  await new ApiTesterRunner(logger)
      //  .CreateConfig(pathConfigJson, ApiTesterConfig);

      //  logger.LogInformation($"Config has been generated.");

      //});

      rootCommand.SetHandler(async (run, version, generateConfig) => {

        if (run) {
          logger.LogInformation($"received a command to start running tests");
          logger.LogInformation($"Validating presence of a config file...");

          if (File.Exists(pathConfigJson)) {
            try {
              await new ApiTesterRunner(logger).RunTests(pathConfigJson);
            } catch (Exception ex) {
              logger.LogInformation($"Failed to run runner Exception.{ex.Message}");

              if (ex.InnerException != null) {
                logger.LogInformation($"Inner exception Exception.{ex.InnerException.Message}");
              }
            }
          } else {
            logger.LogInformation($"Failed to find config on path: {pathConfigJson}");
          }

          logger.LogInformation("");
          logger.LogInformation("Completed test run");
        }

        if (generateConfig) {
          logger.LogInformation($"Started a sample config generation.");

          await new ApiTesterRunner(logger)
          .CreateConfig(pathConfigJson, ApiTesterConfig);

          logger.LogInformation($"Config has been generated.");
          return;
        }

        if (version) {
          logger.LogInformation(Assembly.GetEntryAssembly().GetName().Version.MajorRevision.ToString());
          return;
        }

        return;
      }, run, version, generateConfig);

      _ = await rootCommand.InvokeAsync(args);
    }
  }
}