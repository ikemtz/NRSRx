using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData.Configuration;
using IkeMtz.Samples.OData.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.OData.Tests.Unigration
{
  public class UnigrationODataTestStartup
      : CoreODataUnigrationTestStartup<Startup, ModelConfiguration>
  {
    public override bool IncludeXmlCommentsInSwaggerDocs => true;
    public UnigrationODataTestStartup(IConfiguration configuration) : base(new Startup(configuration))
    {
    }

    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      services.SetupTestDbContext<DatabaseContext>();
    }
  }
}
