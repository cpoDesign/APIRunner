using Microsoft.Extensions.Logging;

namespace APITestingRunner.Unit.Tests
{
    public partial class TestRunnerWithOptionsTests
    {
        public class TestLogger : ILogger
        {
            public List<Tuple<LogLevel, string>> Messages = new();
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotImplementedException();
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                Messages.Add(new Tuple<LogLevel, string>(logLevel, state.ToString()));
            }
        }
















    }
}