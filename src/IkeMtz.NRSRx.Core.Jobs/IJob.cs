using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IkeMtz.NRSRx.Core.Jobs
{
  public interface IJob
  {
    IConfiguration Configuration { get; set; }
    IHost JobHost { get; set; }
    IServiceCollection SetupDependencies(IServiceCollection services);
    IServiceCollection SetupFunctions(IServiceCollection services);
  }
}
