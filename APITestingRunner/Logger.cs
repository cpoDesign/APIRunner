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
            Console.ForegroundColor = ConsoleColor.White;
            switch (logLevel)
            {

                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(state.ToString());
                    break;
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(state.ToString());
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(state.ToString());
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(state.ToString());
                    break;

            }
        }
    }
}