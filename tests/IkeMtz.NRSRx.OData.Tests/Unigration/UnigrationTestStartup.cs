using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.OData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.OData.Tests
{
  public class UnigrationTestStartup
     : CoreODataUnigrationTestStartup<Startup>
  {
    public UnigrationTestStartup(IConfiguration configuration) : base(new Startup(configuration) { MaxTop = 500 })
    {
    }
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }
    public override void SetupDatabase(IServiceCollection services, string dbConnectionString) =>
      services.SetupTestDbContext<DatabaseContext>();
  }
}
