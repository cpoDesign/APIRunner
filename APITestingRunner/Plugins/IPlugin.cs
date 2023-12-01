using APITestingRunner.ApiRequest;
using APITestingRunner.Configuration;
using Microsoft.Extensions.Logging;

namespace APITestingRunner.Plugins
{
    public interface IPlugin
    {
        /// <summary>
        /// Name of the plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        string Description { get; }

        void ApplyConfig(ref IConfig config, ILogger logger);

        /// <summary>
        /// Pre process data before saving happens.
        /// </summary>
        /// <param name="apiResponseString">String to apply plugin onto</param>
        /// <returns>Returns a processed string.</returns>
        string ProcessBeforeSave(string apiResponseString);

        /// <summary>
        /// Compare api call results implementation
        /// </summary>
        /// <param name="apiCallResult"></param>
        /// <param name="fileSourceResult"></param>
        /// <param name="comparisonStatus">Current status of comparing files.</param>
        /// <returns>ComparisonStatus.</returns>
        ComparisonStatus ProcessComparison(ApiCallResult apiCallResult, ApiCallResult fileSourceResult, ComparisonStatus comparisonStatus);

        /// <summary>
        /// Processes validation string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ProcessValidation(string value);
    }
}
