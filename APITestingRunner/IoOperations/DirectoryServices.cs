// See https://aka.ms/new-console-template for more information
using System.Reflection;

public class DirectoryServices
{
    /// <summary>
    /// "C:\\code\\cpoDesign\\APITestingRunner\\APITestingRunner.Unit.Tests\\bin\\Debug\\net7.0"
    /// </summary>
    public static string AssemblyDirectory
  {
        
        get {
      string codeBase = Assembly.GetExecutingAssembly().CodeBase;
      UriBuilder uri = new(codeBase);
      string path = Uri.UnescapeDataString(uri.Path);
      return Path.GetDirectoryName(path);
    }
  }

    public static object SolutionDirectory {
        get {
            return "C:\\code\\cpoDesign\\APITestingRunner\\";
        }
    }
}