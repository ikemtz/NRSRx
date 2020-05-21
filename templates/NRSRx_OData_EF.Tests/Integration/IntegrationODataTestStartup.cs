using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using NRSRx_OData_EF.Configuration;

namespace NRSRx_OData_EF.Tests.Integration
{
  public class IntegrationODataTestStartup
      : CoreODataIntegrationTestStartup<Startup, ModelConfiguration>
  {
    public IntegrationODataTestStartup(IConfiguration configuration)
        : base(new Startup(configuration))
    {
    }
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }
  }
}
