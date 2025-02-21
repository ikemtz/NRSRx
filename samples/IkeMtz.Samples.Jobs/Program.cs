using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Jobs.Core;
using IkeMtz.NRSRx.Jobs.Cron;
using IkeMtz.Samples.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.Jobs
{
  public class Program : JobBase<Program>, IJob
  {
    public static async Task Main()
    {
      var prog = new Program();
      _ = await prog.RunAsync();
    }

    public override IServiceCollection SetupFunctions(IServiceCollection services)
    {
      _ = services.AddFunction<SchoolFunction>();
      _ = services.AddFunction<CourseFunction>();
      return services;
    }
    [ExcludeFromCodeCoverage]
    public override IServiceCollection SetupDependencies(IServiceCollection services)
    {
      return services
        .AddSingleton((_) => TimeProvider.System)
        .AddSingleton<ICronJobStateProvider>((_) => new FileCronJobStateProvider(new DirectoryInfo("cron"), TimeProvider.System))
       .AddDbContext<DatabaseContext>(x => x.UseSqlServer(Configuration.GetValue<string>("DbConnectionString")));
    }

    [ExcludeFromCodeCoverage()]
    public override void SetupLogging(IServiceCollection services)
    {
      _ = this.SetupSplunk(services);
    }
  }
}
