using APITestingRunner.ApiRequest;
using FluentAssertions;

namespace APITestingRunner.Unit.Tests
{

    [TestClass]
    public class DataComparisonTests
    {

		//TODO: Review together tests for any that shoudl be type-safe
        [TestMethod]
        public void CompareAPiResults_ShouldReturnMatching()
        {
            var apiResult = new ApiCallResult(System.Net.HttpStatusCode.OK, string.Empty, null, null, null, true, null) { ResponseContent = string.Empty };
            var fileResult = new ApiCallResult(System.Net.HttpStatusCode.OK, string.Empty, null, null, null, true, null) { ResponseContent = string.Empty };
            var expectedResult = ComparissonStatus.Matching;

            var result = DataComparison.CompareAPiResults(apiResult, fileResult);
            _ = result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CompareAPiResults_ShouldReturnDifferent()
        {
            var apiResult = new ApiCallResult(System.Net.HttpStatusCode.OK, string.Empty, null, null, null, true, null) { ResponseContent = string.Empty };
            var fileResult = new ApiCallResult(System.Net.HttpStatusCode.OK, "test", null, null, null, true, null) { ResponseContent = "test" };
            var expectedResult = ComparissonStatus.Different;


            var result = DataComparison.CompareAPiResults(apiResult, fileResult);
            _ = result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CompareAPiResults_ShouldReturnDifferent_StatusCodeIsDifferent()
        {
            var apiResult = new ApiCallResult(System.Net.HttpStatusCode.OK, string.Empty, null, null, null, true, null) { ResponseContent = string.Empty };
            var fileResult = new ApiCallResult(System.Net.HttpStatusCode.Accepted, "test", null, null, null, true, null) { ResponseContent = "test" };
            var expectedResult = ComparissonStatus.Different;

            var result = DataComparison.CompareAPiResults(apiResult, fileResult);
            _ = result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CompareAPiResults_ShouldReturnDifferent_IsSuccessCodeIsDifferent()
        {
            var apiResult = new ApiCallResult(System.Net.HttpStatusCode.OK, string.Empty, null, null, null, true, null) { ResponseContent = string.Empty };
            var fileResult = new ApiCallResult(System.Net.HttpStatusCode.OK, "test", null, null, null, false, null) { ResponseContent = "test" };
            var expectedResult = ComparissonStatus.Different;

            var result = DataComparison.CompareAPiResults(apiResult, fileResult);
            _ = result.Should().Be(expectedResult);
        }
    }
}