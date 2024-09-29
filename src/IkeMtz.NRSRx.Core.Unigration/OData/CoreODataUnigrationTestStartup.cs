using System.Net.Http;
using IkeMtz.NRSRx.Core.OData;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Authentication;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Abstract base class for setting up an OData unigration test startup.
  /// </summary>
  /// <typeparam name="TStartup">The type of the startup class.</typeparam>
  /// <remarks>
  /// Initializes a new instance of the <see cref="CoreODataUnigrationTestStartup{TStartup}"/> class.
  /// </remarks>
  /// <param name="startup">The startup instance.</param>
  public class CoreODataUnigrationTestStartup<TStartup>(TStartup startup) : CoreODataTestStartup<TStartup>(startup)
          where TStartup : CoreODataStartup
  {

    /// <summary>
    /// Sets up authentication.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }

    /// <summary>
    /// Gets the OpenID configuration.
    /// </summary>
    /// <param name="clientFactory">The HTTP client factory.</param>
    /// <param name="appSettings">The application settings.</param>
    /// <returns>The OpenID configuration.</returns>
    public override OpenIdConfiguration GetOpenIdConfiguration(IHttpClientFactory clientFactory, AppSettings appSettings)
    {
      return new OpenIdConfiguration
      {
        AuthorizeEndpoint = "https://demo.identityserver.io/connect/authorize",
        TokenEndpoint = $"https://demo.identityserver.io/connect/token",
      };
    }
  }
}
