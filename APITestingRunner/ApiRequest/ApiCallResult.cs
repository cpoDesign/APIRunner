// See https://aka.ms/new-console-template for more information

using APITestingRunner.Database;
using System.Net;

namespace APITestingRunner.ApiRequest {

    /// <summary>
    /// Container for an api call result.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="responseContent"></param>
    /// <param name="headers"></param>
    /// <param name="url"></param>
    /// <param name="item"></param>
    /// <param name="IsSuccessStatusCode"></param>
    /// <param name="CompareResults"></param>
    public record ApiCallResult(HttpStatusCode statusCode,
        string responseContent,
        List<KeyValuePair<string, string>> headers,
        string url,
        DataQueryResult? item,
        bool IsSuccessStatusCode,
        List<string>? CompareResults = null) {
    }
}