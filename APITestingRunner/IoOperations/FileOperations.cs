// See https://aka.ms/new-console-template for more information

namespace APITestingRunner.IoOperations
{
    public class FileOperations
    {
        //public static void WriteFile(string path, string name, string content, bool overrideFile) { }

        //public string GetFileContent(string path, string name)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Expected file with extension.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="apiCallResult"></param>
        /// <returns></returns>
        public static async Task WriteFile(string path, string fileContent)
        {
            await File.WriteAllTextAsync(path, fileContent, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Validates if file exists.
        /// </summary>
        /// <param name="fileWithPath">File with path.</param>
        /// <returns>A boolean result.</returns>
        public static bool ValidateIfFileExists(string fileWithPath)
        {
            return File.Exists(fileWithPath);
        }

        /// <summary>
        /// Gets file data for the path.
        /// </summary>
        /// <param name="fileWithPath"></param>
        /// <returns>Content of the file</returns>
        public static string GetFileData(string fileWithPath)
        {
            return File.ReadAllText(fileWithPath, encoding: System.Text.Encoding.UTF8).Trim();
        }
    }
}