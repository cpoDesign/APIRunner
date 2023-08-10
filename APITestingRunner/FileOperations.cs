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

    internal async Task WriteFile(string path, string fileName, ApiCallResult apiCallResult)
    {
      string objString = JsonSerializer.Serialize(apiCallResult);

      await File.WriteAllTextAsync(Path.Combine(path, fileName), objString);
    }
  }
}