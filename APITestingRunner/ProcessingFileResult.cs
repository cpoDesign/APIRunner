// See https://aka.ms/new-console-template for more information

namespace APITestingRunner
{
    public class ProcessingFileResult
    {
        public ComparisonStatus ComparissonStatus { get; set; }
        public bool DisplayFilePathInLog { get; internal set; }
    }

    public enum ComparisonStatus
    {
        NewFile = 1,
        Matching = 2,
        Different = 3,
        NotApplicable = 4,
    }
}