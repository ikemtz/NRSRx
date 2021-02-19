using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IkeMtz.NRSRx.Core.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NRSRx_OData_EF.Data;
using NRSRx_OData_EF.Models;

namespace NRSRx_OData_EF
{
  public class Startup : CoreODataStartup
  {
    public override string MicroServiceTitle => $"{nameof(Item)} OData Microservice";
    public override Assembly StartupAssembly => typeof(Startup).Assembly;

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
