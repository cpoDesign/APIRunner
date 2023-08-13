// See https://aka.ms/new-console-template for more information

public partial class ConfigurationManager
{
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

  public enum RequestType
  {
    GET = 1,
    POST = 2,
    PUT = 3,
    DELETE = 4,
  }
}
