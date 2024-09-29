using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Authentication;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Provides a base class for setting up a unigration test startup for Web API projects.
  /// </summary>
  /// <typeparam name="TStartup">The type of the startup class.</typeparam>
  public class CoreWebApiUnigrationTestStartup<TStartup>(TStartup startup) : CoreWebApiTestStartup<TStartup>(startup)
        where TStartup : CoreWebApiStartup
  {
    /// <summary>
    /// Sets up authentication for unigration tests.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }
  }
}
