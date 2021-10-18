using Microsoft.Extensions.Hosting;

namespace IkeMtz.NRSRx.Logging.ApplicationInsights
{
  public static class HostBuilderExtensions
  {

    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder)
    {
      return hostBuilder;
    }
  }
}
