using System.Runtime.Serialization;

namespace APITestingRunner
{
  [Serializable]
  public class TestRunnerConfigurationErrorsException : Exception
  {
    public TestRunnerConfigurationErrorsException()
    {
    }

    public TestRunnerConfigurationErrorsException(string? message) : base(message)
    {
    }

    public TestRunnerConfigurationErrorsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected TestRunnerConfigurationErrorsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}