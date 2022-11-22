using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Unigration
{
  internal class UnigrationProgram : CoreUnigrationTestJob<Program>
  {
    public UnigrationProgram(Program program, TestContext testContext) : base(program, testContext)
    {
    }

    public override IServiceCollection SetupDependencies(IServiceCollection services) =>
        services.SetupTestDbContext<DatabaseContext>();
  }
}
