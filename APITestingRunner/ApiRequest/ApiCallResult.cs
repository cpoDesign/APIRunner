// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Http.Headers;
using APITestingRunner.Database;

namespace APITestingRunner.ApiRequest
{
    public record ApiCallResult(HttpStatusCode statusCode, string responseContent, HttpResponseHeaders headers, string url, DataQueryResult? item,
      bool IsSuccessStatusCode, List<string>? CompareResults = null)
    {

    }
}