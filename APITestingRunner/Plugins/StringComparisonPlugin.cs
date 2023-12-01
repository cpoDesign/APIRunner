using APITestingRunner.ApiRequest;
using APITestingRunner.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace APITestingRunner.Plugins
{
    public class StringComparisonPlugin : IPlugin
    {
        public string Name => "StringPlugin";

        public string Description => "Compare string into greater detail";

        private ILogger? _logger;
        private IConfig? _config;
        private ComparisonStatus _comparisonStatus;

        public void ApplyConfig(ref IConfig config, ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string ProcessBeforeSave(string apiResponseString)
        {
            return apiResponseString;
        }

        public string ProcessValidation(string value)
        {
            return value;
        }

        public ComparisonStatus ProcessComparison(ApiCallResult apiCallResult, ApiCallResult fileSourceResult, ComparisonStatus comparisonStatus)
        {
            _comparisonStatus = comparisonStatus;
            if ((apiCallResult.StatusCode == fileSourceResult.StatusCode)
                && (apiCallResult.IsSuccessStatusCode == fileSourceResult.IsSuccessStatusCode)
                    && (apiCallResult.ResponseContent == fileSourceResult.ResponseContent))
            {
                _comparisonStatus = ComparisonStatus.Matching;
            }

            if (apiCallResult.ResponseContent == null || fileSourceResult.ResponseContent == null) return _comparisonStatus;
            GetJsonDifferences(apiCallResult.ResponseContent, fileSourceResult.ResponseContent);
            return _comparisonStatus;
        }
        private void GetJsonDifferences(string json1, string json2)
        {
            if (json1 == null && json2 == null)
            {
                return;
            }
            else if (json1 == null && json2 != null)
            {
                return;
            }
            else if (json1 != null && json2 == null)
            {
                return;
            }

            if (json1.Length > json2.Length)
            {
                _comparisonStatus = ComparisonStatus.Different;
                _logger.LogInformation($"Source is different in length: {json1.Length} > {json2.Length}");
            }
            else if (json1.Length < json2.Length)
            {
                _comparisonStatus = ComparisonStatus.Different;
                _logger.LogInformation($"Source is different in length: {json1.Length} < {json2.Length}");
            }
            else
            {
                _logger.LogInformation("Source and target has same length.");
            }

            var token1 = JToken.Parse(json1);
            var token2 = JToken.Parse(json2);
            FindDifferences(token1, token2, string.Empty, "Root");
        }

        private void FindDifferences(JToken token1, JToken token2, string path, string propertyName)
        {
            if (!JToken.DeepEquals(token1, token2))
            {
                _logger.LogInformation($"Difference at path '{path}' for property '{propertyName}'");

                var diffValue = string.Empty;
                var diffValue2 = string.Empty;

                if (token1 != null && token2 != null)
                {
                    if (token1.Type == JTokenType.String || token2.Type == JTokenType.String)
                    {
                        if (token1.Type == JTokenType.String)
                        {
                            diffValue = ((JValue)token1).Value?.ToString();
                        }

                        if (token2.Type == JTokenType.String)
                        {
                            diffValue2 = ((JValue)token2).Value?.ToString();
                        }

                        _comparisonStatus = ComparisonStatus.Different;
                        _logger.LogInformation($"DiffValue is: {diffValue} <> {diffValue2}");
                    }
                }
            }

            if (token1 == null || token2 == null)
            {
                _logger.LogInformation($"Missing path in source at path: '{path}' for property '{propertyName}'");
            }
            else if (token1.Type == JTokenType.Object)
            {
                var props1 = (JObject)token1;
                var props2 = (JObject)token2;

                var allPropertyNames = props1.Properties().Select(p => p.Name).Union(props2.Properties().Select(p => p.Name));

                foreach (var propName in allPropertyNames)
                {
                    FindDifferences(
                        props1.GetValue(propName, StringComparison.OrdinalIgnoreCase),
                        props2.GetValue(propName, StringComparison.OrdinalIgnoreCase),
                        $"{path}.{propName}",
                        propName
                    );
                }
            }
            else if (token1.Type == JTokenType.Array)
            {
                var array1 = (JArray)token1;
                var array2 = (JArray)token2;

                var maxLength = Math.Max(array1.Count, array2.Count);

                for (var i = 0; i < maxLength; i++)
                {
                    var element1 = i < array1.Count ? array1[i] : null;
                    var element2 = i < array2.Count ? array2[i] : null;

                    FindDifferences(element1, element2, $"{path}[{i}]", propertyName);
                }
            }
        }
    }
}