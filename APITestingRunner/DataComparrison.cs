// See https://aka.ms/new-console-template for more information

using APITestingRunner.ApiRequest;

namespace APITestingRunner
{
	public class DataComparrison
    {
        public static ComparissonStatus CompareAPiResults(ApiCallResult apiCallResult, ApiCallResult fileSourceResult)
        {
            var status = ComparissonStatus.Different;

            if ((apiCallResult.statusCode == fileSourceResult.statusCode)
                && (apiCallResult.IsSuccessStatusCode == fileSourceResult.IsSuccessStatusCode)
                    && (apiCallResult.responseContent == fileSourceResult.responseContent))
			{
				status = ComparissonStatus.Matching;
			}

			return status;
        }
    }
}