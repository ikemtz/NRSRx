using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Authentication;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreWebApiUnigrationTestStartup<Startup> : CoreWebApiTestStartup<Startup>
        where Startup : CoreWebApiStartup
  {
    public CoreWebApiUnigrationTestStartup(Startup startup) : base(startup)
    {
    }

    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }
  }
}
