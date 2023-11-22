// See https://aka.ms/new-console-template for more information

using APITestingRunner.ApiRequest;

namespace APITestingRunner
{
    public class DataComparrison
    {
        public static ComparissonStatus CompareAPiResults(ApiCallResult apiCallResult, ApiCallResult fileSourceResult)
        {
            var status = ComparissonStatus.Different;

            if ((apiCallResult.StatusCode == fileSourceResult.StatusCode)
                && (apiCallResult.IsSuccessStatusCode == fileSourceResult.IsSuccessStatusCode)
                    && (apiCallResult.ResponseContent == fileSourceResult.ResponseContent))
            {
                status = ComparissonStatus.Matching;
            }

            return status;
        }
    }
}