// See https://aka.ms/new-console-template for more information
using System.Text.Json;

public partial class ConfigurationManager
{
  public string? LogLocation { get; internal set; }

  public async Task CreateConfig(string pathConfigJson, Config config)
  {

    string objString = JsonSerializer.Serialize(config);
        //TODO: check if file exists and fail if yes.
    await File.WriteAllTextAsync(pathConfigJson, objString);
  }

  public async Task<Config?> GetConfigAsync(string path)
  {
    string fileContent = await File.ReadAllTextAsync(path);
    return string.IsNullOrWhiteSpace(fileContent) ? throw new Exception() : JsonSerializer.Deserialize<Config>(fileContent);
  }
}
