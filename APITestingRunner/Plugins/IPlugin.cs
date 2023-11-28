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
        string Description { get; }
        void ApplyConfig(ref IConfig config, ILogger logger);

        /// <summary>
        /// Pre process data before saving happens.
        /// </summary>
        /// <param name="apiResponseString">String to apply plugin onto</param>
        /// <returns>Returns a processed string.</returns>
        string ProcessBeforeSave(string apiResponseString);

        /// <summary>
        /// Processes validation string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string ProcessValidation(string value);
    }
}
