using System;
using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Abstract base class for setting up an OData integration test startup.
  /// </summary>
  /// <typeparam name="TStartup">The type of the startup class.</typeparam>
  /// <remarks>
  /// Initializes a new instance of the <see cref="CoreODataIntegrationTestStartup{TStartup}"/> class.
  /// </remarks>
  /// <param name="startup">The startup instance.</param>
  public class CoreODataIntegrationTestStartup<TStartup>(TStartup startup) : CoreODataTestStartup<TStartup>(startup)
          where TStartup : CoreODataStartup
  {

    /// <summary>
    /// Sets up authentication.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      Startup.SetupAuthentication(builder);
    }

    /// <summary>
    /// Sets up the database.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="dbConnectionString">The database connection string.</param>
    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      Startup.SetupDatabase(services, !string.IsNullOrWhiteSpace(dbConnectionString) ?
        dbConnectionString : Environment.GetEnvironmentVariable("DbConnectionString"));
    }
  }
}
