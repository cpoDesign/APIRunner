using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace APITestingRunner.Unit.Tests {

  public class TestLogger : ILogger {
    public List<Tuple<LogLevel, string>> Messages = new();
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
      throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel) {
      throw new NotImplementedException();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) {
      if (logLevel == LogLevel.Information) {
        Messages.Add(new Tuple<LogLevel, string>(logLevel, state.ToString()));
      } else {
        Debug.WriteLine(state.ToString());
      }
    }
  }
}