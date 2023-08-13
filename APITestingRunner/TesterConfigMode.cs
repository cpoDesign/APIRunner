// See https://aka.ms/new-console-template for more information

public partial class ConfigurationManager
{
  public enum TesterConfigMode
  {
    /// <summary>
    /// Runs the tests only and shows result as overview.
    /// </summary>
    Run = 1,
    /// <summary>
    /// Runs the tests and capture the results.
    /// </summary>
    Capture = 2,
    /// <summary>
    /// Calls APIs and compare to a stored file.
    /// </summary>
    FileCompare = 3,
    /// <summary>
    /// Realtime compare. Compares the results of two APIs. 
    /// Good for regression testing of APIs.
    /// </summary>
    APICompare = 4
  }
}
