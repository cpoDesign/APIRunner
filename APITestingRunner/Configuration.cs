// See https://aka.ms/new-console-template for more information
using System.Text.Json;

public class Configuration
{
  public async Task CreateConfig(string path)
  {
    Config config = new()
    {
      UrlBase = "http://localhost:5152/",
      UrlPath = "/Data",
      UrlParam = new List<Param>
      {
        new Param("urlKey", "test"),
        new Param("id", "sqlId")
      },
      HeaderParam = new List<Param> {
        new Param("accept","application/json")
      },
      DBConnectionString = "Server=127.0.0.1; Database=test; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True;",
      DBQuery = "select id as sqlId from dbo.sampleTable;",
      DBFields = new List<Param>
      {
        new Param("sqlId", "sqlId")
      },
      RequestType = "GET",
      ResultsStoreOption = StoreResultsOption.All,


    };

    string objString = JsonSerializer.Serialize(config);

    await File.WriteAllTextAsync(path, objString);
  }

  public async Task<Config?> GetConfigAsync(string path)
  {
    string fileContent = await File.ReadAllTextAsync(path);
    return string.IsNullOrWhiteSpace(fileContent) ? throw new Exception() : JsonSerializer.Deserialize<Config>(fileContent);
  }

  public class Config
  {
    public required string UrlBase { get; set; }
    public required List<Param> UrlParam { get; set; }
    public required List<Param> HeaderParam { get; set; }
    public string? DBConnectionString { get; set; }
    public string? DBQuery { get; set; }
    public List<Param>? DBFields { get; set; }
    public required string RequestType { get; set; }
    public required string UrlPath { get; set; }
    public StoreResultsOption ResultsStoreOption { get; set; }
  }

  public enum StoreResultsOption
  {
    /// <summary>
    /// Just run the tests
    /// </summary>
    None = 0,
    /// <summary>
    /// stores all results
    /// </summary>
    All = 1,
    /// <summary>
    /// Record only failures
    /// </summary>
    FailuresOnly = 2,
  }

  public record Param(string Name, string value);
}
