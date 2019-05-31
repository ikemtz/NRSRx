using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace IkeMtz.NRSRx.Core.Unigration.Logging
{
    public class TestContextLogger : ILogger
    {
        private readonly TestContext testContext;
        private readonly string categoryName;
        public TestContextLogger(string categoryName, TestContext testContext)
        {
            this.testContext = testContext;
            this.categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            try
            {
                testContext.WriteLine($"**{categoryName}** Beginning State: {state?.ToString()}");
            }
            catch (Exception)
            {

                testContext.WriteLine($"**{categoryName}** Beginning State: Too Large To specify");
            }
            return new TestContextOperationScope<TState>(this.testContext, state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                testContext.WriteLine(formatter(state, exception));
            }
            else
            {
                testContext.WriteLine($"**{categoryName}** {logLevel}:");
                testContext.WriteLine($"State: {state?.ToString()}");
                if (exception != null)
                {
                    testContext.WriteLine($"Exception: {JsonConvert.SerializeObject(exception, Constants.JsonSerializerSettings)}");
                }
            }
        }
    }
}
