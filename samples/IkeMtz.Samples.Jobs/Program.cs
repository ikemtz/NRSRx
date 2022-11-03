using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.Jobs;
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
      _ = services.AddSingleton<IFunction, FirstFunction>();
      _ = services.AddSingleton<IFunction, SecondFunction>();
      return services;
    }

    [ExcludeFromCodeCoverage()]
    public override void SetupLogging(IServiceCollection services)
    {
      _ = this.SetupSplunk(services);
    }
  }
}
