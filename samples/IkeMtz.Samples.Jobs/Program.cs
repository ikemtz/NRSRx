using IkeMtz.NRSRx.Core.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.Jobs
{
  public class Program : Job
  {
    public override string Name => nameof(IkeMtz.Samples.Jobs);
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
    public override void SetupLogging(IServiceCollection services)
    {
      _ = this.SetupSplunk(services);
    }
  }
}
