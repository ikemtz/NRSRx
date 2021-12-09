using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using IkeMtz.Samples.WebApi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.WebApi
{
  public class Startup : CoreWebApiStartup
  {
    public override string MicroServiceTitle => $"{nameof(Samples)} WebApi Microservice";
    public override Assembly StartupAssembly => typeof(Startup).Assembly;
    public override bool IncludeXmlCommentsInSwaggerDocs => true;

    public Startup(IConfiguration configuration) : base(configuration) { }

    [ExcludeFromCodeCoverage]
    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      _ = services
      .AddDbContext<DatabaseContext>(x => x
        .UseSqlServer(dbConnectionString)
        .EnableDetailedErrors()
        );
    }
    public override void SetupLogging(IServiceCollection services = null, IApplicationBuilder app = null) =>
      this.SetupElasticsearch(app);
  }
}
