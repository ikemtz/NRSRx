using System.Reflection;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NRSRx_WebApi_EF.Data;

namespace NRSRx_WebApi_EF.WebApi
{
  public class Startup : CoreWebApiStartup
  {

    public override string MicroServiceTitle => $"{nameof(NRSRx_WebApi_EF)} WebApi Microservice";
    public override Assembly StartupAssembly => typeof(Startup).Assembly;

    public Startup(IConfiguration configuration) : base(configuration) { }


    public override void SetupDatabase(IServiceCollection services, string connectionString)
    {
      _ = services
      .AddDbContext<DatabaseContext>(x => x.UseSqlServer(connectionString))
      .AddEntityFrameworkSqlServer();
    }

    public override void SetupMiscDependencies(IServiceCollection services)
    {
      _ = services.AddScoped<IDatabaseContext, DatabaseContext>();
    }
  }
}
