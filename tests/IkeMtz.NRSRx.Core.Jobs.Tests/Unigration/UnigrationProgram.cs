using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Jobs.Cron;
using IkeMtz.NRSRx.Jobs.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Jobs.Tests.Unigration
{
  internal class UnigrationProgram : CoreUnigrationTestJob<Program>
  {
    public UnigrationProgram(Program program, TestContext testContext) : base(program, testContext)
    {
      RunContinously = false;
    }

    public override IServiceCollection SetupDependencies(IServiceCollection services) =>
        services
          .AddSingleton((_) => TimeProvider.System)
          .AddSingleton<ICronJobStateProvider, MockCronJobStateProvider>()
          .SetupTestDbContext<DatabaseContext>()
      ;
  }
}
