// See https://aka.ms/new-console-template for more information

using System.Text.Json;

namespace APITestingRunner
{
  public class FileOperations
  {
    public void WriteFile(string path, string name, string content, bool overrideFile) { }
    public string GetFileContent(string path, string name)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Expected file with extension.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <param name="apiCallResult"></param>
    /// <returns></returns>
    internal async Task WriteFile(string path, string fileName, ApiCallResult apiCallResult)
    {
      string objString = JsonSerializer.Serialize(apiCallResult);

      await File.WriteAllTextAsync(Path.Combine(path, fileName), objString);
    }
  }
}