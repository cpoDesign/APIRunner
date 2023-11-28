
using System.Diagnostics;
using System.Reflection;

namespace APITestingRunner.Unit.Tests
{
    /// <summary>
    /// TODO refactor shared code into this class
    /// </summary>
    public class TestBase
    {
        public string _dbConnectionStringForTests = string.Empty;

        internal void Initialize()
        {
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            _dbConnectionStringForTests = DatabaseConnectionBuilder.BuildFileDatabaseConnection(filePath, "SampleDb.mdf");

            // TODO: for now update this related to your checkout location and support LUT
            Debug.WriteLine(_dbConnectionStringForTests);
        }
    }
}