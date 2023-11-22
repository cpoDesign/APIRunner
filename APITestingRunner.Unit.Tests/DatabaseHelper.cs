using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APITestingRunner.Unit.Tests
{
	internal class DatabaseHelper
	{
		public static string BuildLocalhostDatabaseConnection(string server = "127.0.0.1", string dbName = "test", string userId = "sa", string password = "<YourStrong@Passw0rd>", bool trustServerCertificate = true)
		{
			// "Server=127.0.0.1; Database=test; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True;",
			var dbConnection = $"Server={server}; Database={dbName}; User Id={userId}; Password={password}; TrustServerCertificate={trustServerCertificate};";
			return dbConnection;
		}

		public static string BuildFileDatabaseConnection(string fileLocation, string fileName)
		{
			var dbConnection = "(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\code\\cpoDesign\\APITestingRunner\\APITestingRunner.Unit.Tests\\SampleDb.mdf;Integrated Security = True";
			// var dbConnection = $"(LocalDB)\\MSSQLLocalDB;AttachDbFilename={fileLocation}{fileName}; Integrated Security=True";
			return dbConnection;
		}
	}
}
