// See https://aka.ms/new-console-template for more information
using System.Text.Json;

namespace APITestingRunner.Configuration
{
	public class ConfigurationManager
	{
		public static async Task CreateConfig(string pathConfigJson, Config config)
		{
			var objString = JsonSerializer.Serialize(config);

			if (File.Exists(pathConfigJson))
				File.Delete(pathConfigJson);

			await File.WriteAllTextAsync(pathConfigJson, objString);
		}

		public static async Task<Config?> GetConfigAsync(string path)
		{
			var fileContent = await File.ReadAllTextAsync(path);
			return string.IsNullOrWhiteSpace(fileContent) ? throw new Exception() : JsonSerializer.Deserialize<Config>(fileContent);
		}
	}
}