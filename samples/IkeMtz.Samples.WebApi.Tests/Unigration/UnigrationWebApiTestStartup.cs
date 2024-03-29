using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.WebApi.Tests.Unigration
{
  public class UnigrationWebApiTestStartup
      : CoreWebApiUnigrationTestStartup<Startup>
  {
    public UnigrationWebApiTestStartup(IConfiguration configuration)
        : base(new Startup(configuration))
    {
    }
    public override void SetupDatabase(IServiceCollection services, string dbConnectionString) =>
      services.SetupTestDbContext<DatabaseContext>();
  }
}
