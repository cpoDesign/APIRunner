using APITestingRunner.ApiRequest;
using FluentAssertions;

namespace APITestingRunner.Unit.Tests
{

    [TestClass]
    public class DataComparrisonTests
    {

        [TestMethod]
        public void CompareAPiResults_ShouldReturnMatching()
        {
            ApiCallResult apiResult = new ApiCallResult(System.Net.HttpStatusCode.OK, "", null, null, null, true, null);

            ApiCallResult fileResult = new ApiCallResult(System.Net.HttpStatusCode.OK, "", null, null, null, true, null);
            ComparissonStatus expectedResult = ComparissonStatus.Matching;


            var result = DataComparrison.CompareAPiResults(apiResult, fileResult);
            _ = result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CompareAPiResults_ShouldReturnDifferent()
        {
            ApiCallResult apiResult = new ApiCallResult(System.Net.HttpStatusCode.OK, "", null, null, null, true, null);
            ApiCallResult fileResult = new ApiCallResult(System.Net.HttpStatusCode.OK, "test", null, null, null, true, null);
            ComparissonStatus expectedResult = ComparissonStatus.Different;


            var result = DataComparrison.CompareAPiResults(apiResult, fileResult);
            _ = result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CompareAPiResults_ShouldReturnDifferent_StatusCodeIsDifferent()
        {
            ApiCallResult apiResult = new ApiCallResult(System.Net.HttpStatusCode.OK, "", null, null, null, true, null);
            ApiCallResult fileResult = new ApiCallResult(System.Net.HttpStatusCode.Accepted, "test", null, null, null, true, null);
            ComparissonStatus expectedResult = ComparissonStatus.Different;

            var result = DataComparrison.CompareAPiResults(apiResult, fileResult);
            _ = result.Should().Be(expectedResult);
        }

        [TestMethod]
        public void CompareAPiResults_ShouldReturnDifferent_IsSuccessCodeIsDifferent()
        {
            ApiCallResult apiResult = new ApiCallResult(System.Net.HttpStatusCode.OK, "", null, null, null, true, null);
            ApiCallResult fileResult = new ApiCallResult(System.Net.HttpStatusCode.OK, "test", null, null, null, false, null);
            ComparissonStatus expectedResult = ComparissonStatus.Different;

            var result = DataComparrison.CompareAPiResults(apiResult, fileResult);
            _ = result.Should().Be(expectedResult);
        }
    }
}