using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration.Logging
{
  public static class TestContextLoggerExtensions
  { 
    public static ILoggingBuilder AddTestContext(this ILoggingBuilder builder, TestContext testContext)
    {
      builder
          .SetMinimumLevel(LogLevel.Trace)
          .Services
          .Add(ServiceDescriptor.Singleton<ILoggerProvider, TestContextLoggerProvider>((x) => new TestContextLoggerProvider(testContext)));
      return builder;
    }
  }
}
