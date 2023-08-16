// See https://aka.ms/new-console-template for more information

public partial class ConfigurationManager
{
  /// <summary>
  /// Options to to store the results for the response
  /// </summary>
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
}
