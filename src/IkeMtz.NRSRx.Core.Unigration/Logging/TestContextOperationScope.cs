using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace IkeMtz.NRSRx.Core.Unigration.Logging
{
    public sealed class TestContextOperationScope<TState> : IDisposable
    {
        private readonly TestContext testContext;
        private readonly Stopwatch stopWatch;
        private readonly TState state;
        public TestContextOperationScope(TestContext testContext, TState state)
        {
            this.testContext = testContext;
            this.state = state;
            this.stopWatch = new Stopwatch();
            this.stopWatch.Start();
            this.testContext.WriteLine($"Starting Scope: {state.ToString()} @ {DateTime.Now.ToShortDateString()}");
        }

        public void Dispose()
        {
            this.stopWatch.Stop();
            this.testContext.WriteLine($"Ending Scope: {state.ToString()} Total Milliseconds: {this.stopWatch.ElapsedMilliseconds}");
        }
    }
}
