using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.OData.Tests.Unigration
{
  public class UnigrationODataTestStartup
      : CoreODataUnigrationTestStartup<Startup>
  {
    public UnigrationODataTestStartup(IConfiguration configuration) : base(new Startup(configuration))
    {
    }

    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      services.SetupTestDbContext<DatabaseContext>();
    }
  }
}
