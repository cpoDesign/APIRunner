// See https://aka.ms/new-console-template for more information

using APITestingRunner.ApiRequest;
using APITestingRunner.Configuration;
using APITestingRunner.Plugins;
using Microsoft.Extensions.Logging;

namespace APITestingRunner
{
    /// <summary>
    /// Simple data comparison plugin.
    /// </summary>
    public class APICallResultComparison : IPlugin
    {
        /// <inheritdoc/>
        public string Name => "APICallResultStatus";

        /// <inheritdoc/>
        public string Description => "Validates if API call has been successful as the source API result";

        /// <inheritdoc/>
        public void ApplyConfig(ref IConfig config, ILogger logger)
        {

        }

        /// <inheritdoc/>
        public string ProcessBeforeSave(string apiResponseString)
        {
            return apiResponseString;
        }

        /// <inheritdoc/>
        public ComparisonStatus ProcessComparison(ApiCallResult apiCallResult, ApiCallResult fileSourceResult, ComparisonStatus comparisonStatus)
        {

            return comparisonStatus;
        }

        /// <inheritdoc/>
        public string ProcessValidation(string value)
        {
            return value;
        }
    }
}