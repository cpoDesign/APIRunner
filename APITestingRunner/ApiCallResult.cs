// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Http.Headers;

namespace APITestingRunner
{
  public record ApiCallResult(HttpStatusCode statusCode, string responseContent, HttpResponseHeaders headers, string url, DataQueryResult? item)
  {

  }
}