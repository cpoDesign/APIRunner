using System.Text;
using WireMock;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace APITestingRunner.Unit.Tests.TestRunnerTests
{
    public class CustomResponse : IResponseProvider
    {
        private static int _count = 0;
        public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, IRequestMessage requestMessage, WireMockServerSettings settings)
        {
            ResponseMessage response;
            if (_count % 2 == 0)
            {
                response = new ResponseMessage() { StatusCode = 200 };
                SetBody(response, @"{ ""msg"": ""Hello from wiremock!"" }");
            }
            else
            {
                response = new ResponseMessage() { StatusCode = 500 };
                SetBody(response, @"{ ""msg"": ""Hello some error from wiremock!"" }");
            }

            _count++;
            (ResponseMessage, IMapping) tuple = (response, null);
            return await Task.FromResult(tuple);
        }


        private void SetBody(ResponseMessage response, string body)
        {
            response.BodyDestination = BodyDestinationFormat.SameAsSource;
            response.BodyData = new BodyData
            {
                Encoding = Encoding.UTF8,
                DetectedBodyType = BodyType.String,
                BodyAsString = body
            };
        }
    }
}