// See https://aka.ms/new-console-template for more information

namespace APITestingRunner.IoOperations {
    public class FileOperations {
        public void WriteFile(string path, string name, string content, bool overrideFile) { }
        public string GetFileContent(string path, string name) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Expected file with extension.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="apiCallResult"></param>
        /// <returns></returns>
        internal async Task WriteFile(string path, string fileContent) {

            await File.WriteAllTextAsync(path, fileContent);
        }

        internal bool ValidateIfFileExists(string fileName) {
            return File.Exists(fileName);
        }

        public static string GetFileData(string filePath) {
            return File.ReadAllText(filePath, encoding: System.Text.Encoding.UTF8).Trim();
        }

    }
}