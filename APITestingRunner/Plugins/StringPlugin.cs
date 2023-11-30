using Newtonsoft.Json.Linq;

namespace APITestingRunner.Plugins
{
    public class StringPlugin
    {
        public static List<string> CompareStrings(string string1, string string2)
        {
            var differencesResult = new List<string>();
            _ = GetJsonDifferences(string1, string2, differencesResult);

            return differencesResult;
        }

        private static List<string> GetJsonDifferences(string json1, string json2, List<string> differences)
        {
            var token1 = JToken.Parse(json1);
            var token2 = JToken.Parse(json2);

            if (json1.Length > json2.Length)
            {
                differences.Add($"Source is different in length: {json1.Length} > {json2.Length}");
            }
            else if (json1.Length < json2.Length)
            {
                differences.Add($"Source is different in length: {json1.Length} < {json2.Length}");
            }

            FindDifferences(token1, token2, differences, string.Empty, "Root");
            return differences;
        }

        private static void FindDifferences(JToken token1, JToken token2, List<string> differences, string path, string propertyName)
        {
            if (!JToken.DeepEquals(token1, token2))
            {
                differences.Add($"Difference at path '{path}' for property '{propertyName}'");

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

                        differences.Add($"DiffValue is: {diffValue} <> {diffValue2}");
                    }
                }
            }

            if (token1 == null || token2 == null)
            {
                differences.Add($"Missing path in source at path: '{path}' for property '{propertyName}'");
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
                        differences,
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

                    FindDifferences(element1, element2, differences, $"{path}[{i}]", propertyName);
                }
            }
        }
    }
}