namespace APITestingRunner.Exceptions
{
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

		// TODO: Review - https://learn.microsoft.com/en-us/dotnet/fundamentals/syslib-diagnostics/syslib0051#workaround
		//protected TestRunnerConfigurationErrorsException(SerializationInfo info, StreamingContext context) : base(info, context)
		//{
		//}
	}
}