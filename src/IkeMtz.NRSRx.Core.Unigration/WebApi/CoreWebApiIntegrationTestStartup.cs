using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi
{
  /// <summary>
  /// Provides a startup class for integration tests of Web API projects.
  /// </summary>
  /// <typeparam name="TStartup">The type of the startup class.</typeparam>
  public class CoreWebApiIntegrationTestStartup<TStartup>(TStartup startup) : CoreWebApiTestStartup<TStartup>(startup)
          where TStartup : CoreWebApiStartup
  {

    /// <summary>
    /// Sets up authentication for the integration tests.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      Startup.SetupAuthentication(builder);
    }

    /// <summary>
    /// Sets up the database for the integration tests.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="dbConnectionString">The database connection string.</param>
    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      Startup.SetupDatabase(services, dbConnectionString);
    }

    /// <summary>
    /// Sets up publishers for the integration tests.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public override void SetupPublishers(IServiceCollection services)
    {
      Startup.SetupPublishers(services);
    }
  }
}
