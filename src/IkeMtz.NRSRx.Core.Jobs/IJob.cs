using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public interface IJob
  {
    IConfiguration Configuration { get; }
    IHost JobHost { get; }
    string Name { get; }

    IServiceCollection SetupDependencies(IServiceCollection services);
    IServiceCollection SetupJobs(IServiceCollection services);
  }
}