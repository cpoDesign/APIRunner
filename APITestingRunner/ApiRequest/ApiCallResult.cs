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
	public record ApiCallResult(HttpStatusCode StatusCode,
		string ResponseContent,
        List<KeyValuePair<string, string>> Headers,
        string Url,
        DataQueryResult? Item,
        bool IsSuccessStatusCode,
        List<string>? CompareResults = null)    {
    }
}