using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData.Configuration;
using IkeMtz.Samples.OData.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IkeMtz.Samples.OData.Tests.Unigration
{
  public class UnigrationODataTestStartup
      : CoreODataUnigrationTestStartup<Startup, ModelConfiguration>
  {
    public UnigrationODataTestStartup(IConfiguration configuration) : base(new Startup(configuration))
    {
    }

    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      services.SetupTestDbContext<DatabaseContext>();
    }
    public override void SetupSwaggerGen(SwaggerGenOptions options, string xmlPath = null)
    {
      var path = StartupAssembly.CodeBase
        .Replace(".dll", ".xml", System.StringComparison.InvariantCultureIgnoreCase)
        //This is here to work around an issue on Azure Devops build agents not finding the .xml file.
        .Replace(@"\$(BuildConfiguration)\netcoreapp3.1\IkeMtz.Samples.OData.xml", @"\Debug\netcoreapp3.1\IkeMtz.Samples.OData.xml", System.StringComparison.InvariantCultureIgnoreCase)
        ;
      base.SetupSwaggerGen(options, path);
    }
  }
}
