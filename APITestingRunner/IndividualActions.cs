// See https://aka.ms/new-console-template for more information

//Console.WriteLine("Hello, World!");


//Console.WriteLine(path);
//Console.ReadLine();

using Microsoft.Extensions.Logging;

namespace APITestingRunner {
    public class IndividualActions {
        private ILogger _logger;

        public IndividualActions AddLogger(ILogger logger) {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            return this;
        }

        public async Task CreateConfig(string directory, string pathConfigJson, Config config) {
            ConfigurationManager configManager = new();

            Console.WriteLine($"Created config on path: {pathConfigJson}");
            await configManager.CreateConfig(pathConfigJson, config);
            return;

        }
        public async Task RunTests(string pathConfigJson) {
            Console.WriteLine($"Loading config on path: {pathConfigJson}");

            ConfigurationManager configManager = new();

            Config? configSettings = await configManager.GetConfigAsync(pathConfigJson);
            TestRunner testRunner = new(_logger);
            await testRunner.ApplyConfig(configSettings);

            testRunner = await testRunner.RunTestsAsync();

            _ = await testRunner.PrintResultsSummary();
            return;
        }

        public async Task<TestRunner> RunTests(Config config) {
            TestRunner testRunner = new(_logger);
            await testRunner.ApplyConfig(config);

            return await testRunner.RunTestsAsync();

        }
    }
}