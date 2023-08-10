// See https://aka.ms/new-console-template for more information

//Console.WriteLine("Hello, World!");


//Console.WriteLine(path);
//Console.ReadLine();

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

      Console.WriteLine("At the prompt, enter text as if it were a string of command line arguments. Enter 'exit' to exit.");

      string pathConfigJson = $"{DirectoryServices.AssemblyDirectory}\\config.json";

      Configuration config = new()
      {
        LogLocation = DirectoryServices.AssemblyDirectory
      };

      while (true)
      {
        Console.Write("> ");

        string input = Console.ReadLine();


        if (input == "exit")
          break;

        CreateConfig = true;


        if (CreateConfig)
        {
          await IndividualActions.CreateConfig(DirectoryServices.AssemblyDirectory, pathConfigJson);
        }

        await IndividualActions.RunTests(pathConfigJson);

      }
    }
  }

  public class IndividualActions
  {
    public static async Task CreateConfig(string directory, string pathConfigJson)
    {

      Configuration config = new();
      {
        Console.WriteLine($"Created config on path: {pathConfigJson}");
        await config.CreateConfig(directory, pathConfigJson);
        return;
      }
    }
    public static async Task RunTests(string pathConfigJson)
    {
      Console.WriteLine($"Loading config on path: {pathConfigJson}");

      Configuration config = new();

      Configuration.Config? configSettings = await config.GetConfigAsync(pathConfigJson);
      TestRunner testRunner = new();
      testRunner = await testRunner.ApplyConfig(configSettings)
                .RunTestsAsync();

      testRunner = await testRunner.GetTestRunnerDbSet();

      testRunner = await testRunner.RunTestsAsync();

      _ = await testRunner.PrintResults();
      return;
    }
  }
}