using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IkeMtz.NRSRx.Core.OData;
using IkeMtz.Samples.OData.Data;
using IkeMtz.Samples.OData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.OData
{
  public class Startup : CoreODataStartup
  {
    public override string MicroServiceTitle => $"{nameof(Item)} OData Microservice";
    public override Assembly StartupAssembly => typeof(Startup).Assembly;
    public override bool IncludeXmlCommentsInSwaggerDocs => true;

    public Startup(IConfiguration configuration) : base(configuration)
    {
    }

    [ExcludeFromCodeCoverage]
    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      _ = services
       .AddDbContextPool<DatabaseContext>(x => x.UseSqlServer(dbConnectionString));
    }

    public override void SetupMiscDependencies(IServiceCollection services)
    {
      _ = services.AddScoped<IDatabaseContext, DatabaseContext>();
    }
  }
}
