// See https://aka.ms/new-console-template for more information

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
    /// Calls APIs and store result. If file already exists then it wil also compare output from a api with stored file.
    /// </summary>
    CaptureAndCompare = 3,

    ///// <summary>
    ///// TODO Implement first
    ///Realtime compare. Compares the results of two APIs. 
    ///// Good for regression testing of APIs.
    ///// </summary>
    //APICompare = 4
}