using Microsoft.Extensions.Hosting;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Provides extension methods for configuring logging on the host builder.
  /// </summary>
  public static class HostBuilderExtensions
  {
    /// <summary>
    /// Configures the host builder to use logging.
    /// </summary>
    /// <param name="hostBuilder">The host builder to configure.</param>
    /// <returns>The configured host builder.</returns>
    public static IHostBuilder UseLogging(this IHostBuilder hostBuilder)
    {
      return hostBuilder;
    }
  }
}
