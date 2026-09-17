using System;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Provides extension methods for <see cref="IHostBuilder"/> used when constructing
  /// test hosts for integration or unigration tests.
  /// </summary>
  /// <remarks>
  /// These helpers forward configuration to the underlying web host builder and
  /// enable configuring test-specific services via <see cref="TestServer"/> helpers.
  /// </remarks>
  public static class IHostBuilderExtensions
  {
    /// <summary>
    /// Configures test services for the web host built by the provided <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">
    /// The host builder to configure. This is the instance that will be used to build the host.
    /// </param>
    /// <param name="servicesConfiguration">
    /// A delegate that receives an <see cref="IServiceCollection"/> and configures services
    /// for testing. This is forwarded to the web host's <c>ConfigureTestServices</c> method.
    /// </param>
    /// <returns>
    /// The same <see cref="IHostBuilder"/> instance so that additional configuration can be chained.
    /// </returns>
    public static IHostBuilder ConfigureTestServices(this IHostBuilder hostBuilder, Action<IServiceCollection> servicesConfiguration)
    {
      return hostBuilder.ConfigureWebHost(webBuilder =>
          webBuilder.ConfigureTestServices(servicesConfiguration));
    }

    /// <summary>
    /// Configures services for the web host built by the provided <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">
    /// The host builder to configure. This is the instance that will be used to build the host.
    /// </param>
    /// <param name="servicesConfiguration">
    /// A delegate that receives an <see cref="IServiceCollection"/> used to register services
    /// for the application. This configuration is forwarded to the web host's <c>ConfigureServices</c> method.
    /// </param>
    /// <returns>
    /// The same <see cref="IHostBuilder"/> instance so that additional configuration can be chained.
    /// </returns>
    /// <remarks>
    /// This method is a convenience that forwards the provided <paramref name="servicesConfiguration"/>
    /// to the underlying web host builder. It enables configuring services at the host-builder
    /// level when creating test hosts.
    /// </remarks>
    public static IHostBuilder ConfigureServices(this IHostBuilder hostBuilder, Action<IServiceCollection> servicesConfiguration)
    {
      return hostBuilder.ConfigureWebHost(webBuilder =>
          webBuilder.ConfigureServices(servicesConfiguration));
    }
  }
}
