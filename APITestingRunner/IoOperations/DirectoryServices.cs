// See https://aka.ms/new-console-template for more information
using System.Reflection;

public class DirectoryServices
{
  public static string AssemblyDirectory
  {
    get
    {
      string codeBase = Assembly.GetExecutingAssembly().CodeBase;
      UriBuilder uri = new(codeBase);
      string path = Uri.UnescapeDataString(uri.Path);
      return Path.GetDirectoryName(path);
    }
  }
}