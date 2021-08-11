using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration.Logging
{
  public sealed class TestContextLoggerProvider : ILoggerProvider
  {
    private readonly TestContext testContext;
    private readonly ConcurrentDictionary<string, TestContextLogger> _loggers = new();

    public TestContextLoggerProvider(TestContext testContext)
    {
      this.testContext = testContext;
    }

    public ILogger CreateLogger(string categoryName)
    {
      return _loggers.GetOrAdd(categoryName, name => new TestContextLogger(name, testContext));
    }

    public void Dispose()
    {
      _loggers.Clear();
    }
  }
}
