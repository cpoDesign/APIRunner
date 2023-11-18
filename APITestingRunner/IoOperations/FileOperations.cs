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

        public static string GetFileChecksum(string filePath) {


            return File.ReadAllText(filePath).Trim();
            //return CreateMD5(File.ReadAllText(filePath));
        }

        public static string CreateMD5(string input) {
            return input.Trim();
            //// Use input string to calculate MD5 hash
            //using (MD5 md5 = MD5.Create()) {
            //    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input.Trim());
            //    byte[] hashBytes = md5.ComputeHash(inputBytes);

            //    return Convert.ToHexString(hashBytes);

            //}
        }
    }
}