using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.OData;
using IkeMtz.Samples.OData.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.OData.Tests
{
  public  class IntegrationTestStartup
     : CoreODataIntegrationTestStartup<Startup, ModelConfiguration>
  {
    public override bool IncludeXmlCommentsInSwaggerDocs => true;
    public IntegrationTestStartup(IConfiguration configuration) : base(new Startup(configuration))
    {
    }
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }
  }
}
