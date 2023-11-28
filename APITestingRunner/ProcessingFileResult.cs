// See https://aka.ms/new-console-template for more information

namespace APITestingRunner
{
    public class ProcessingFileResult
    {
        public ComparissonStatus ComparissonStatus { get; set; }
        public bool DisplayFilePathInLog { get; internal set; }
    }

    public enum ComparissonStatus
    {
        NewFile = 1,
        Matching = 2,
        Different = 3,
    }
}