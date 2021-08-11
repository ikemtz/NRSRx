using IkeMtz.NRSRx.Core.Unigration.WebApi;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class UnitTestStartup : CoreWebApiIntegrationTestStartup<StartUp_AppInsights>
  {
    public UnitTestStartup(IConfiguration configuration) : base(new StartUp_AppInsights(configuration))
    {
    }
  }
}
