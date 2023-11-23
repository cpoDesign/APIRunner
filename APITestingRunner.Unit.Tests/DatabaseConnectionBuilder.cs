using FluentAssertions.Equivalency;
using System.Diagnostics;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APITestingRunner.Unit.Tests
{
	internal class DatabaseConnectionBuilder
	{
		public static string BuildLocalhostDatabaseConnection(string server = "127.0.0.1", string dbName = "test", string userId = "sa", string password = "<YourStrong@Passw0rd>", bool trustServerCertificate = true)
		{
			// "Server=127.0.0.1; Database=test; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True;",
			return $"Server={server}; Database={dbName}; User Id={userId}; Password={password}; TrustServerCertificate={trustServerCertificate};";
		}

		public static string BuildFileDatabaseConnection(string fileLocation, string fileName)
		{
            Debug.WriteLine($"|||DEBUG||| BuildFileDatabaseConnection - fileLocation: {fileLocation}, fileName: {fileName}");
			var buildpath = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={fileLocation}\\{fileName}; Integrated Security=True";
			var dbConnection = buildpath;
			Debug.WriteLine($"|||DEBUG||| BuildFileDatabaseConnection - dbConnection: {dbConnection}");
			return dbConnection;
		}
	}
}
