using Microsoft.Extensions.Hosting;

namespace IkeMtz.NRSRx.Core.Web
{
  public static class HostBuilderExtensions
  {

    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder)
    {
      return hostBuilder;
    }
  }
}
