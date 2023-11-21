// See https://aka.ms/new-console-template for more information
using System.Reflection;

public class DirectoryServices
{
    /// <summary>
    /// "C:\\code\\cpoDesign\\APITestingRunner\\APITestingRunner.Unit.Tests\\bin\\Debug\\net7.0"
    /// </summary>
    public static string AssemblyDirectory
    {

        get
        {
#pragma warning disable SYSLIB0012 // Type or member is obsolete
            string codeBase = Assembly.GetExecutingAssembly().CodeBase!;
#pragma warning restore SYSLIB0012 // Type or member is obsolete

            UriBuilder uri = new(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path)!;
        }
    }
}