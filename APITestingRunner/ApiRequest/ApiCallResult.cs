// See https://aka.ms/new-console-template for more information

using APITestingRunner.Database;
using System.Net;

namespace APITestingRunner.ApiRequest
{

    /// <summary>
    /// Container for an api call result.
    /// </summary>
    /// <param name="StatusCode"></param>
    /// <param name="ResponseContent"></param>
    /// <param name="Headers"></param>
    /// <param name="Url"></param>
    /// <param name="Item"></param>
    /// <param name="IsSuccessStatusCode"></param>
    /// <param name="CompareResults"></param>
    public record ApiCallResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseContent { get; set; }
        public List<KeyValuePair<string, string>>? Headers { get; set; }
        public string? Url { get; set; }
        public DataQueryResult? DataQueryResult { get; set; }
        public bool IsSuccessStatusCode { get; set; }
        public List<string>? CompareResults { get; set; }
    }
}