using APITestingRunner.IoOperations;
using System.Diagnostics;

namespace APITestingRunner.Unit.Tests.IOTests
{
    [TestClass]
    public class LanguageSpecificTests
    {
        [TestMethod]
        [TestCategory("multiLanguage")]
        [DataRow("你好，你怎麼樣", "ChineseTraditional")]
        [DataRow("你好，你怎么样", "ChineseTimplified")]
        public async Task LanguageGetsStoredCorrectlyInFileAsync(string contentOfTheFile, string lanagueName)
        {
            var directory = DirectoryServices.AssemblyDirectory;

            var fileName = Path.Combine(directory, $"{lanagueName}.json");
            //apiContentResponse = Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(apiContentResponse));

            await FileOperations.WriteFile(fileName, contentOfTheFile);
            Debug.WriteLine(fileName);

            var fileResult = FileOperations.GetFileData(fileName);
            Assert.AreEqual(contentOfTheFile, fileResult);
        }
    }
}
