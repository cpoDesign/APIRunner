// See https://aka.ms/new-console-template for more information
using System.Text.Json;

public partial class ConfigurationManager
{
  public string? LogLocation { get; internal set; }

  public async Task CreateConfig(string pathConfigJson, Config config)
  {

    string objString = JsonSerializer.Serialize(config);

    await File.WriteAllTextAsync(pathConfigJson, objString);
  }

  public async Task<Config?> GetConfigAsync(string path)
  {
    string fileContent = await File.ReadAllTextAsync(path);
    return string.IsNullOrWhiteSpace(fileContent) ? throw new Exception() : JsonSerializer.Deserialize<Config>(fileContent);
  }
}
