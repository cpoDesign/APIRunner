// See https://aka.ms/new-console-template for more information

using APITestingRunner.ApiRequest;

namespace APITestingRunner.Configuration
{
    public interface IConfig
    {
        string? CompareUrlBase { get; set; }
        string? CompareUrlPath { get; set; }
        TesterConfigMode? ConfigMode { get; set; }
        string? DBConnectionString { get; set; }
        List<Param>? DBFields { get; set; }
        string? DBQuery { get; set; }
        List<Param> HeaderParam { get; set; }
        string? OutputLocation { get; set; }
        string? RequestBody { get; set; }
        RequestType RequestType { get; set; }
        string? ResultFileNamePattern { get; set; }
        StoreResultsOption ResultsStoreOption { get; set; }
        string UrlBase { get; set; }
        List<Param> UrlParam { get; set; }
        string UrlPath { get; set; }
        /// <summary>
        /// IPlugin: Contains definitons for content replacements.
        /// </summary>
        public ContentReplacement[]? ContentReplacements { get; set; }
    }
}