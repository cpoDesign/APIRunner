using APITestingRunner.Configuration;
using Microsoft.Extensions.Logging;

namespace APITestingRunner.Plugins
{

    /// <summary>
    /// Allow user to apply replacements as part of file storing and comparison.
    /// </summary>
    public class ContentReplacements() : IPlugin
    {
        private IConfig _config;
        private readonly ILogger<ContentReplacements> logger;

        public string Description => "Allow user to apply replacements as part of file storing and comparison.";

        string IPlugin.Name => "ContentReplacements";


        //TODO: Review this pattern - need reason not to put this into the constructor - which improves the code tightness
        public void ApplyConfig(ref IConfig config, ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(config, nameof(config));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            _config = config;
        }

        public string ProcessValidation(string value)
        {
            return ProcessValueWithFilters(false, value);
        }

        public string ProcessBeforeSave(string value)
        {
            return ProcessValueWithFilters(true, value);
        }

        private string ProcessValueWithFilters(bool filterConfigurationByStoreInFile, string value)
        {
            foreach (var replacementConfig in ApplySavedFilter(filterConfigurationByStoreInFile))
            {
                value = value.Replace(replacementConfig.From, replacementConfig.To);
            }

            return value;
        }

        private List<ContentReplacement> ApplySavedFilter(bool filterConfigurationByStoreInFile)
        {
            if (_config.ContentReplacements == null)
                return [];

            if (!filterConfigurationByStoreInFile)
                return _config.ContentReplacements.ToList();

            return _config.ContentReplacements.Where(x => x.StoreInFile).ToList();
        }
    }
}
