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
              
                await IndividualActions.RunTests(pathConfigJson);

                Console.WriteLine("======================");
            }

            Console.WriteLine("completed run");
            _ = Console.ReadKey();
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

            _ = await testRunner.PrintResultsSummary();
            return;
        }

        public static async Task RunTests(Config config)
        {
            TestRunner testRunner = new();
            await testRunner.ApplyConfig(config);


            // execute db data load only has some data in it
            if (!string.IsNullOrWhiteSpace(config.DBConnectionString) && !string.IsNullOrWhiteSpace(config.DBQuery) && config.DBFields.Count() > 0)
            {
                testRunner = await testRunner.GetTestRunnerDbSet();
            }

            testRunner = await testRunner.RunTestsAsync();

            _ = await testRunner.PrintResultsSummary();
            return;
        }
    }
}