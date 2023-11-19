// See https://aka.ms/new-console-template for more information


using Microsoft.Extensions.Logging;
using System.CommandLine;
using static ConfigurationManager;

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
      DBConnectionString = "sql server database config",
      DBQuery = "select id as bindingId from dbo.sampleTable;",
      DBFields = new List<Param>
             {
                    new Param("bindingId", "bindingId"),
                  },
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
       ("--generateConfig", "an option to generate a new config file with sample data.");

      var config = new Option<string>
       ("--config", "A path to a custom config file to be used for the runner.");

      var run = new Option<bool>
       ("--run", "Run the tester.");

      var rootCommand = new RootCommand("Parameter binding example");
      //rootCommand.Add(delayOption);
      //rootCommand.Add(messageOption);

      //rootCommand.SetHandler(
      //    (delayOptionValue, messageOptionValue) => {
      //      DisplayIntAndString(delayOptionValue, messageOptionValue);
      //    },
      //    delayOption, messageOption);

      rootCommand.Add(config);
      rootCommand.Add(generateConfig);
      rootCommand.Add(run);


      string pathConfigJson = $"{DirectoryServices.AssemblyDirectory}\\config.json";


      rootCommand.SetHandler(async (generateConfig) => await new ApiTesterRunner(logger).CreateConfig(pathConfigJson, ApiTesterConfig), generateConfig);

      rootCommand.SetHandler(async (run, config) => {
        if (run) {
          if (File.Exists(config)) {
            await new ApiTesterRunner(logger).RunTests(config);
          } else {
            await new ApiTesterRunner(logger).RunTests(pathConfigJson);
          }
        }
      }, run, config);


      _ = await rootCommand.InvokeAsync(args);
    }

    //public static void DisplayIntAndString(int delayOptionValue, string messageOptionValue) {
    //  Console.WriteLine($"--delay = {delayOptionValue}");
    //  Console.WriteLine($"--message = {messageOptionValue}");
    //}

    //public static Task DisplayIntAndString(bool run, string configPath) {

    //  await new ApiTesterRunner().RunTests(configPath);
    //}
  }
}