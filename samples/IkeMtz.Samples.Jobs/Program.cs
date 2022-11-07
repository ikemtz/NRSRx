using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Jobs;
using IkeMtz.Samples.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.Jobs
{
  public class Program : Job<Program>
  {
    public static async Task Main()
    {
      var prog = new Program();
      await prog.RunAsync();
    }

    public override IServiceCollection SetupFunctions(IServiceCollection services)
    {
      _ = services.AddSingleton<IFunction, SchoolFunction>();
      _ = services.AddSingleton<IFunction, CourseFunction>();
      return services;
    }
    [ExcludeFromCodeCoverage]
    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      return services
       .AddSingleton<ICurrentUserProvider, SystemUserProvider>()
       .AddDbContext<DatabaseContext>(x => x.UseSqlServer(Configuration.GetValue<string>("DbConnectionString")));
    }

    [ExcludeFromCodeCoverage()]
    public override void SetupLogging(IServiceCollection services)
    {
      _ = this.SetupSplunk(services);
    }
  }
}
