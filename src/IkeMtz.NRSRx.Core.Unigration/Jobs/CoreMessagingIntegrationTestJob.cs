using IkeMtz.NRSRx.Core.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration.Jobs
{
  public class CoreMessagingIntegrationTestJob<TProgram> : CoreMessagingUnigrationTestJob<TProgram>
        where TProgram : class, IJob
  {
    public CoreMessagingIntegrationTestJob(TProgram program, TestContext testContext) : base(program, testContext)
    {
    }

    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      Program.Configuration = this.Configuration;
      return base.SetupDependencies(services);
    }
  }
}
