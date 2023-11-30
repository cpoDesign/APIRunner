using APITestingRunner.Plugins;
using FluentAssertions;

namespace APITestingRunner.Unit.Tests.Plugins
{
    [TestClass]
    public class StringPluginTests
    {
        [TestMethod]
        public void NoDifferencesReported()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            var json2 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";

            var resultDifferences = StringPlugin.CompareStrings(json1, json2);
            _ = resultDifferences.Should().BeEmpty();
        }

        [TestMethod]
        public void DifferentLength_isReported()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\",\"country\":\"USA\"}";

            var resultDifferences = StringPlugin.CompareStrings(json1, json2);
            _ = resultDifferences.Should().HaveCountGreaterThan(1);
            _ = resultDifferences.Should().ContainMatch("Source is different in length: 42 < 58");
        }

        [TestMethod]
        public void DifferentLength_KeyMissingIsReported_FromTarget()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\",\"country\":\"USA\"}";

            var resultDifferences = StringPlugin.CompareStrings(json1, json2);
            _ = resultDifferences.Should().HaveCountGreaterThan(1);
            _ = resultDifferences.Should().ContainMatch("Difference at path '.country' for property 'country'");
            _ = resultDifferences.Should().ContainMatch("Missing path in source at path: '.country' for property 'country'");
        }

        [TestMethod]
        public void DifferentLength_KeyMissingIsReported_FromSource()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\", \"country\":\"USA\"}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\"}";

            var resultDifferences = StringPlugin.CompareStrings(json1, json2);
            _ = resultDifferences.Should().HaveCountGreaterThan(1);
            _ = resultDifferences.Should().ContainMatch("Difference at path '.country' for property 'country'");
            _ = resultDifferences.Should().ContainMatch("Missing path in source at path: '.country' for property 'country'");
        }

        [TestMethod]
        public void Different_DifferentValueIsReported()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\"}";
            var json2 = "{\"name\":\"John\",\"age\":30,\"city\":\"New Yorks\"}";

            var resultDifferences = StringPlugin.CompareStrings(json1, json2);
            _ = resultDifferences.Should().HaveCount(4);
            _ = resultDifferences.Should().ContainMatch("Difference at path '' for property 'Root'");
            _ = resultDifferences.Should().ContainMatch("Difference at path '.city' for property 'city'");
            _ = resultDifferences.Should().ContainMatch("DiffValue is: New York <> New Yorks");
        }
        [TestMethod]
        public void StringPlugin_ArrayValues()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\",\"hobbies\":[\"reading\",\"traveling\"]}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\",\"hobbies\":[\"reading\",\"cooking\"]}";

            var resultDifferences = StringPlugin.CompareStrings(json1, json2);
            _ = resultDifferences.Should().HaveCount(5);
            _ = resultDifferences.Should().ContainMatch("Difference at path '.hobbies' for property 'hobbies'");
            _ = resultDifferences.Should().ContainMatch("Difference at path '.hobbies[1]' for property 'hobbies'");
            _ = resultDifferences.Should().ContainMatch("DiffValue is: traveling <> cooking");
        }

        [TestMethod]
        public void Different_DeepArrayNestingComparison()
        {
            var json1 = "{\"name\":\"John\",\"age\":30,\"city\":\"New York\",\"hobbies\":[{\"type\":\"reading\",\"locations\":[{\"name\":\"Library\",\"hours\":9},{\"name\":\"Park\",\"hours\":5}]}]}";
            var json2 = "{\"age\":30,\"city\":\"New York\",\"name\":\"John\",\"hobbies\":[{\"type\":\"reading\",\"locations\":[{\"name\":\"Library\",\"hours\":9},{\"name\":\"Beach\",\"hours\":8}]}]}";

            var resultDifferences = StringPlugin.CompareStrings(json1, json2);
            _ = resultDifferences.Should().HaveCount(9);

            _ = resultDifferences.Should().ContainMatch("Difference at path '.hobbies' for property 'hobbies'");
            _ = resultDifferences.Should().ContainMatch("Difference at path '.hobbies[0]' for property 'hobbies'");
            _ = resultDifferences.Should().ContainMatch("Difference at path '.hobbies[0].locations' for property 'locations'");
            _ = resultDifferences.Should().ContainMatch("Difference at path '.hobbies[0].locations[1]' for property 'locations'");
            _ = resultDifferences.Should().ContainMatch("Difference at path '.hobbies[0].locations[1].name' for property 'name'");
            _ = resultDifferences.Should().ContainMatch("DiffValue is: Park <> Beach");
            _ = resultDifferences.Should().ContainMatch("Difference at path '.hobbies[0].locations[1].hours' for property 'hours'");
        }
    }
}
