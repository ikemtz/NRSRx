using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.Jobs;
using IkeMtz.Samples.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.Jobs
{
  public class Program : Job
  {
    public static async Task Main()
    {
      var prog = new Program();
      await prog.RunAsync();
    }

    public override IServiceCollection SetupJobs(IServiceCollection services)
    {
      _ = services.AddSingleton<IFunction, SchoolFunction>();
      _ = services.AddSingleton<IFunction, CourseFunction>();
      return services;
    }

    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      return services
       .AddDbContextPool<DatabaseContext>(x => x.UseSqlServer(Configuration.GetValue<string>("DbConnectionString")));
    }

    [ExcludeFromCodeCoverage()]
    public override void SetupLogging(IServiceCollection services)
    {
      _ = this.SetupSplunk(services);
    }
  }
}
