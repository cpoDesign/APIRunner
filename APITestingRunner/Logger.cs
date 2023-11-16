// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging;

namespace APITestingRunner
{
    public class Logger : ILogger
    {
        internal static void LogOutput(string logMessage)
        {
            Console.WriteLine(logMessage);
        }

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
            throw new NotImplementedException();
        }
    }
}