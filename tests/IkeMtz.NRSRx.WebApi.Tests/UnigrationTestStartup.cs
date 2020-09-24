using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.WebApi;
using IkeMtz.Samples.WebApi.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.WebApi.Tests
{
  public class UnigrationTestStartup : CoreWebApiUnigrationTestStartup<Startup>
  {
    public override bool IncludeXmlCommentsInSwaggerDocs => true;
    public UnigrationTestStartup(IConfiguration configuration) : base(new Startup(configuration))
    {
    }
    public override void SetupDatabase(IServiceCollection services, string connectionString)
    {
      services.SetupTestDbContext<DatabaseContext>();
    }
  }
}
