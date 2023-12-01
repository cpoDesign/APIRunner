using APITestingRunner.ApiRequest;
using FluentAssertions;

namespace APITestingRunner.Unit.Tests.TestRunnerTests
{

    [TestClass]
    public class DataComparisonTests
    {

        //TODO: Review together tests for any that shoudl be type-safe
        [TestMethod]
        public void CompareAPiResults_ShouldReturnMatching()
        {
            // ->  StatusCode = response.StatusCode, Headers = responseHeaders, Url = pathAndQuery, Item = item, IsSuccessStatusCode = response.IsSuccessStatusCode, ResponseContent = content }
            var apiResult = new ApiCallResult { StatusCode = System.Net.HttpStatusCode.OK, Headers = null, Url = null, DataQueryResult = null, IsSuccessStatusCode = true, ResponseContent = string.Empty };
            var fileResult = new ApiCallResult { StatusCode = System.Net.HttpStatusCode.OK, Headers = null, Url = null, DataQueryResult = null, IsSuccessStatusCode = true, ResponseContent = string.Empty };
            var expectedResult = ComparisonStatus.Matching;

            var result = new APICallResultComparison().ProcessComparison(apiResult, fileResult, ComparisonStatus.NewFile);
            _ = result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CompareAPiResults_ShouldReturnDifferent()
        {
            var apiResult = new ApiCallResult { StatusCode = System.Net.HttpStatusCode.OK, Headers = null, Url = null, DataQueryResult = null, IsSuccessStatusCode = true, ResponseContent = string.Empty };
            var fileResult = new ApiCallResult { StatusCode = System.Net.HttpStatusCode.OK, Headers = null, Url = null, DataQueryResult = null, IsSuccessStatusCode = true, ResponseContent = "test" };
            var expectedResult = ComparisonStatus.Different;


            var result = new APICallResultComparison().ProcessComparison(apiResult, fileResult, ComparisonStatus.NewFile);
            _ = result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CompareAPiResults_ShouldReturnDifferent_StatusCodeIsDifferent()
        {
            var apiResult = new ApiCallResult { StatusCode = System.Net.HttpStatusCode.OK, Headers = null, Url = null, DataQueryResult = null, IsSuccessStatusCode = true, ResponseContent = string.Empty };
            var fileResult = new ApiCallResult { StatusCode = System.Net.HttpStatusCode.Accepted, Headers = null, Url = null, DataQueryResult = null, IsSuccessStatusCode = true, ResponseContent = "test" };
            var expectedResult = ComparisonStatus.Different;

            var result = new APICallResultComparison().ProcessComparison(apiResult, fileResult, ComparisonStatus.NewFile);
            _ = result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CompareAPiResults_ShouldReturnDifferent_IsSuccessCodeIsDifferent()
        {
            var apiResult = new ApiCallResult { StatusCode = System.Net.HttpStatusCode.OK, Headers = null, Url = null, DataQueryResult = null, IsSuccessStatusCode = false, ResponseContent = "test" };
            var fileResult = new ApiCallResult { StatusCode = System.Net.HttpStatusCode.OK, Headers = null, Url = null, DataQueryResult = null, IsSuccessStatusCode = true, ResponseContent = "test" };
            var expectedResult = ComparisonStatus.Different;

            var result = new APICallResultComparison().ProcessComparison(apiResult, fileResult, ComparisonStatus.NewFile);
            _ = result.Should().Be(expectedResult);
        }
    }
}