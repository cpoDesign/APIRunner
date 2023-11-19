// See https://aka.ms/new-console-template for more information


using System.CommandLine;

namespace APITestingRunner {
  /// <summary>
  ///     Provides an eval/print loop for command line argument strings.
  ///     TODO: implement command line binder https://learn.microsoft.com/en-us/dotnet/standard/commandline/model-binding
  /// </summary>
  internal static class Program {

    private static bool CreateConfig { get; set; }

    private static bool RunTest { get; set; }

    private static bool Help { get; set; }


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
      var delayOption = new Option<int>
       ("--delay", "An option whose argument is parsed as an int.");
      var messageOption = new Option<string>
          ("--message", "An option whose argument is parsed as a string.");

      var rootCommand = new RootCommand("Parameter binding example");
      rootCommand.Add(delayOption);
      rootCommand.Add(messageOption);

      rootCommand.SetHandler(
          (delayOptionValue, messageOptionValue) => {
            DisplayIntAndString(delayOptionValue, messageOptionValue);
          },
          delayOption, messageOption);

      _ = await rootCommand.InvokeAsync(args);
      //  while (true) {
      //    Console.Write(">> ");

      //    string input = Console.ReadLine()!;

      //    if (input == "exit")
      //      break;

      //    string pathConfigJson = $"{DirectoryServices.AssemblyDirectory}\\config.json";


      //    await new ApiTesterRunner().RunTests(pathConfigJson);

      //    Console.WriteLine("======================");
      //  }

      //  Console.WriteLine("completed run");
      //  _ = Console.ReadKey();
      //}
    }
    public static void DisplayIntAndString(int delayOptionValue, string messageOptionValue) {
      Console.WriteLine($"--delay = {delayOptionValue}");
      Console.WriteLine($"--message = {messageOptionValue}");
    }
  }
}