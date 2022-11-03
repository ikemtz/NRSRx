using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Unigration.Fakes
{
  public class FakeLogger : ILogger
  {
    public FakeLogger()
    {
      this.Logs = new List<(LogLevel LogLevel, string Message)>();
    }

    public IDisposable BeginScope<TState>(TState state)
    {
      throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      return true;
    }

    public List<(LogLevel LogLevel, string State)> Logs { get; }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      Logs.Add((logLevel, state?.ToString() ?? throw new NullReferenceException("State should not be null")));
    }
  }
}
