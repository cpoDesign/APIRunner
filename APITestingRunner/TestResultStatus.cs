// See https://aka.ms/new-console-template for more information

namespace APITestingRunner
{
    public class TestResultStatus
    {
        public int StatusCode { get; set; }
        public int NumberOfResults { get; set; } = 0;
    };

    public class TestConstants
    {
        /// <summary>
        /// Name of the test directory.
        /// </summary>
        public static readonly string TestOutputDirectory = "Results";
    }
}