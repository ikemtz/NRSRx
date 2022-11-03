using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IkeMtz.Samples.Jobs;
using Microsoft.Extensions.DependencyInjection;
using IkeMtz.NRSRx.Core.Unigration.Logging;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Unigration
{
  internal class UnigrationProgram : Program
  {
    public UnigrationProgram(TestContext testContext)
    {
      TestContext = testContext;
    }

    public TestContext TestContext { get; }

    public override void SetupLogging(IServiceCollection services)
    {
      _ = services.AddLogging(x => x.AddTestContext(TestContext));
    }
  }
}
