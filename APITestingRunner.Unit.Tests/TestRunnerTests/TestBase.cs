
using System.Diagnostics;

namespace APITestingRunner.Unit.Tests {
    /// <summary>
    /// TODO refactor shared code into this class
    /// </summary>
    public class TestBase {

        public readonly string _dbConnectionStringForTests = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\code\\cpoDesign\\APITestingRunner\\APITestingRunner.Unit.Tests\\SampleDb.mdf;Integrated Security=True";

        internal void Initialize() {
            // TODO: for now update this related to your checkout location and support LUT
            Debug.WriteLine(_dbConnectionStringForTests);
        }
    }
}