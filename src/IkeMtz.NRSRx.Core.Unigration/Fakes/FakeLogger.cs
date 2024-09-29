using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Unigration.Fakes
{
  /// <summary>
  /// A fake logger implementation for testing purposes.
  /// </summary>
  public class FakeLogger : ILogger
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeLogger"/> class.
    /// </summary>
    public FakeLogger()
    {
      this.Logs = [];
    }

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
    /// <param name="state">The identifier for the scope.</param>
    /// <returns>An <see cref="IDisposable"/> that ends the logical operation scope on dispose.</returns>
    /// <exception cref="NotImplementedException">Always thrown as this method is not implemented.</exception>
    public IDisposable BeginScope<TState>(TState state)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns>Always returns true as all log levels are enabled.</returns>
    public bool IsEnabled(LogLevel logLevel)
    {
      return true;
    }

    /// <summary>
    /// Gets the list of log entries.
    /// </summary>
    public List<(LogLevel LogLevel, string State)> Logs { get; }

    /// <summary>
    /// Writes a log entry.
    /// </summary>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    /// <param name="logLevel">The log level.</param>
    /// <param name="eventId">The event ID.</param>
    /// <param name="state">The state object.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="formatter">The function to create a log message.</param>
    /// <exception cref="NullReferenceException">Thrown if the state is null.</exception>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      Logs.Add((logLevel, state?.ToString() ?? throw new NullReferenceException("State should not be null")));
    }
  }
}
