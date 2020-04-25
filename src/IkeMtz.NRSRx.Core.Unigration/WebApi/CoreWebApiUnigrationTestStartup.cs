using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Authentication;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreWebApiUnigrationTestStartup<TStartup> : CoreWebApiTestStartup<TStartup>
        where TStartup : CoreWebApiStartup
  {
    public CoreWebApiUnigrationTestStartup(TStartup startup) : base(startup)
    {
    }

    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }
  }
}
