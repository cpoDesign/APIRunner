// See https://aka.ms/new-console-template for more information

//Console.WriteLine("Hello, World!");


//Console.WriteLine(path);
//Console.ReadLine();

using static ConfigurationManager;

namespace APITestingRunner
{
  /// <summary>
  ///     Provides an eval/print loop for command line argument strings.
  /// </summary>
  internal static class Program
  {
    private static bool CreateConfig { get; set; }

    private static bool RunTest { get; set; }

    private static bool Help { get; set; }


    /// <summary>
    ///     Returns a "pretty" string representation of the provided Type; specifically, corrects the naming of generic Types
    ///     and appends the type parameters for the type to the name as it appears in the code editor.
    /// </summary>
    /// <param name="type">The type for which the colloquial name should be created.</param>
    /// <returns>A "pretty" string representation of the provided Type.</returns>
    public static string ToColloquialString(this Type type)
    {
      return !type.IsGenericType ? type.Name : type.Name.Split('`')[0] + "<" + string.Join(", ", type.GetGenericArguments().Select(a => a.ToColloquialString())) + ">";
    }

    /// <summary>
    ///     Application entry point
    /// </summary>
    /// <param name="args">Command line arguments</param>
    private static async Task Main(string[] args)
    {
      // enable ctrl+c
      Console.CancelKeyPress += (o, e) =>
      {
        Environment.Exit(1);
      };


      while (true)
      {
        Console.Write("> ");

        string input = Console.ReadLine();

        if (input == "exit")
          break;

        string pathConfigJson = $"{DirectoryServices.AssemblyDirectory}\\config.json";
        Console.WriteLine("==========CreateConfigForSingleAPICall============");

        //CreateConfigForSingleAPICall(pathConfigJson);

        //await IndividualActions.RunTests(pathConfigJson);

        //Console.ReadLine();

        //Console.WriteLine("==========CreateConfigForSingleAPICallWithUrlParam============");

        //CreateConfigForSingleAPICallWithUrlParam(pathConfigJson);

        //await IndividualActions.RunTests(pathConfigJson);


        //Console.ReadLine();

        //Console.WriteLine("==========CreateConfigForDatabaseBasedAPICall============");

        //CreateConfigForDatabaseBasedAPICall(pathConfigJson);

        //await IndividualActions.RunTests(pathConfigJson);

        //Console.WriteLine("======================");


        //Console.ReadLine();

        //Console.WriteLine("==========CreateConfigForDatabaseBasedAPICall============");

        //CreateConfigForDatabaseBasedAPICall(pathConfigJson, StoreResultsOption.None);

        //await IndividualActions.RunTests(pathConfigJson);

        //Console.WriteLine("======================");


        //Console.ReadLine();

        //Console.WriteLine("==========CreateConfigForDatabaseBasedAPICall with capture mode============");

        //CreateConfigForDatabaseBasedAPICallWithFailures(pathConfigJson, StoreResultsOption.FailuresOnly);
        //await IndividualActions.RunTests(pathConfigJson);
        //Console.WriteLine("======================");

        //Console.ReadLine();

        //Console.WriteLine("==========CreateConfigForDatabaseBasedAPIComparrisonCall============");

        //CreateConfigForDatabaseBasedAPIComparrisonCall(pathConfigJson, StoreResultsOption.None);

        //await IndividualActions.RunTests(pathConfigJson);

        //Console.WriteLine("======================");

        Console.ReadLine();

        Console.WriteLine("==========CreateConfigForDatabaseBasedAPIComparrisonCall============");

        CreateConfigForSingleAPICallWithUrlParamAndBodyModel(pathConfigJson);

        await IndividualActions.RunTests(pathConfigJson);

        Console.WriteLine("======================");
      }


      Console.WriteLine("completed run");
      _ = Console.ReadKey();
    }

    private static void CreateConfigForSingleAPICall(string pathConfigJson)
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

      _ = IndividualActions.CreateConfig(DirectoryServices.AssemblyDirectory, pathConfigJson, config);
    }

    private static void CreateConfigForSingleAPICallWithUrlParam(string pathConfigJson)
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

      _ = IndividualActions.CreateConfig(DirectoryServices.AssemblyDirectory, pathConfigJson, config);
    }


    private static void CreateConfigForDatabaseBasedAPICall(string pathConfigJson, StoreResultsOption storeResultsOption = StoreResultsOption.None)
    {
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
        DBConnectionString = "Server=127.0.0.1; Database=test; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True;",
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

      _ = IndividualActions.CreateConfig(DirectoryServices.AssemblyDirectory, pathConfigJson, config);
    }

    private static void CreateConfigForDatabaseBasedAPICallWithFailures(string pathConfigJson, StoreResultsOption storeResultsOption = StoreResultsOption.None)
    {
      Config config = new()
      {
        UrlBase = "https://localhost:7055",
        CompareUrlBase = string.Empty,
        CompareUrlPath = string.Empty,
        UrlPath = "/WithFailure",
        UrlParam = new List<Param>
      {
        new Param("urlKey", "test"),
        new Param("id", "sqlId")
      },
        HeaderParam = new List<Param> {
        new Param("accept","application/json")
      },
        RequestBody = null,
        DBConnectionString = "Server=127.0.0.1; Database=test; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True;",
        DBQuery = "select id as sqlId from dbo.sampleTable;",
        DBFields = new List<Param>
      {
        new Param("sqlId", "sqlId")
      },
        RequestType = RequestType.GET,
        ResultsStoreOption = storeResultsOption,
        ConfigMode = TesterConfigMode.Capture,
        LogLocation = DirectoryServices.AssemblyDirectory
      };

      _ = IndividualActions.CreateConfig(DirectoryServices.AssemblyDirectory, pathConfigJson, config);
    }

    private static void CreateConfigForDatabaseBasedAPIComparrisonCall(string pathConfigJson, StoreResultsOption storeResultsOption = StoreResultsOption.None)
    {
      Config config = new()
      {
        UrlBase = "https://localhost:7055",
        CompareUrlBase = "https://localhost:7055",
        UrlPath = "/Data",
        CompareUrlPath = "/DataV2",
        UrlParam = new List<Param>
      {
        new Param("urlKey", "test"),
        new Param("id", "sqlId")
      },
        HeaderParam = new List<Param> {
        new Param("accept","application/json")
      },
        RequestBody = null,
        DBConnectionString = "Server=127.0.0.1; Database=test; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True;",
        DBQuery = "select id as sqlId from dbo.sampleTable;",
        DBFields = new List<Param>
      {
        new Param("sqlId", "sqlId")
      },
        RequestType = RequestType.GET,
        ResultsStoreOption = storeResultsOption,
        ConfigMode = TesterConfigMode.APICompare,
        LogLocation = DirectoryServices.AssemblyDirectory
      };

      _ = IndividualActions.CreateConfig(DirectoryServices.AssemblyDirectory, pathConfigJson, config);
    }


    private static void CreateConfigForSingleAPICallWithUrlParamAndBodyModel(string pathConfigJson)
    {
      Config config = new()
      {
        UrlBase = "https://localhost:7055",
        CompareUrlBase = string.Empty,
        CompareUrlPath = string.Empty,
        UrlPath = "/datamodel/123456789",
        UrlParam = new List<Param>
        {
          new Param("location","UK")
        },
        HeaderParam = new List<Param> {
        new Param("accept","application/json")
      },
        RequestBody = "{Id={sqlId},StaticData=\"data\"}",
        DBConnectionString = null,
        DBQuery = null,
        DBFields = null,
        RequestType = RequestType.GET,
        ResultsStoreOption = StoreResultsOption.None,
        ConfigMode = TesterConfigMode.Run,
        LogLocation = DirectoryServices.AssemblyDirectory
      };

      _ = IndividualActions.CreateConfig(DirectoryServices.AssemblyDirectory, pathConfigJson, config);
    }
  }

  public class IndividualActions
  {
    public static async Task CreateConfig(string directory, string pathConfigJson, Config config)
    {
      ConfigurationManager configManager = new();

      Console.WriteLine($"Created config on path: {pathConfigJson}");
      await configManager.CreateConfig(pathConfigJson, config);
      return;

    }
    public static async Task RunTests(string pathConfigJson)
    {
      Console.WriteLine($"Loading config on path: {pathConfigJson}");

      ConfigurationManager configManager = new();

      Config? configSettings = await configManager.GetConfigAsync(pathConfigJson);
      TestRunner testRunner = new();
      await testRunner.ApplyConfig(configSettings);


      // execute db data load only has some data in it
      if (!string.IsNullOrWhiteSpace(configSettings.DBConnectionString) && !string.IsNullOrWhiteSpace(configSettings.DBQuery) && configSettings.DBFields.Count() > 0)
      {
        testRunner = await testRunner.GetTestRunnerDbSet();
      }

      testRunner = await testRunner.RunTestsAsync();

      _ = await testRunner.PrintResults();
      return;
    }
  }
}