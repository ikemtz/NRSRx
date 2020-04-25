using IkeMtz.NRSRx.Core.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Authentication;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreODataUnigrationTestStartup<TStartup, TModelConfiguration>
        : CoreODataTestStartup<TStartup, TModelConfiguration>
        where TStartup : CoreODataStartup
        where TModelConfiguration : IModelConfiguration, new()
  {
    public CoreODataUnigrationTestStartup(TStartup startup) : base(startup)
    {
    }

    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }
  }
}
