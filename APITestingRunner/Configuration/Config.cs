// See https://aka.ms/new-console-template for more information

using static ConfigurationManager;

public class Config {
    /// <summary>
    /// Base path of the url.
    /// </summary>
    public required string UrlBase { get; set; }
    /// <summary>
    /// Alternative compare url in case of option of comparing requests.
    /// </summary>
    public required string? CompareUrlBase { get; set; }

    /// <summary>
    /// Relative path for the client.
    /// </summary>
    public required string UrlPath { get; set; }

    /// <summary>
    /// Contains a compare url path. Allows user to point to the same api under different name.
    /// </summary>
    public required string? CompareUrlPath { get; set; }

    /// <summary>
    /// Any query parameters api requires.
    /// </summary>
    public required List<Param> UrlParam { get; set; }
    /// <summary>
    /// Any headers the api requires
    /// </summary>
    public required List<Param> HeaderParam { get; set; }

    /// <summary>
    /// Request body optional. When present the replace values will apply the same way like for url.
    /// </summary>
    public string? RequestBody { get; set; }

    /// <summary>
    /// What type of HTTP verb to use for the API call.
    /// </summary>
    public required RequestType RequestType { get; set; }

    /// <summary>
    /// What is the config for the runner.
    /// </summary>
    public required TesterConfigMode? ConfigMode { get; set; }

    /// <summary>
    /// What to do with results
    /// </summary>
    public StoreResultsOption ResultsStoreOption { get; set; }

    /// <summary>
    /// Location where responses will be stored.
    /// Can be null and if yes no responses will be stored even if StoreResults is enabled
    /// </summary>
    public string? OutputLocation { get; set; }

    /// <summary>
    /// Database connection string to target your database source.
    /// </summary>
    public string? DBConnectionString { get; set; }

    /// <summary>
    /// Query to generate the data to use for data generation for api arguments.
    /// </summary>
    public string? DBQuery { get; set; }

    /// <summary>
    /// Database fields mapping to parameters
    /// </summary>
    public List<Param>? DBFields { get; set; }

    /// <summary>
    /// Result file name pattern used to create a result file when used in combination of result capture.
    /// </summary>
    public string? ResultFileNamePattern { get; set; }
}
