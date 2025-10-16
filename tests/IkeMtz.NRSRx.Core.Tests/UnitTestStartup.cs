using IkeMtz.NRSRx.Core.Unigration.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [DoNotParallelize]
  public class UnitTestStartup(IConfiguration configuration) : CoreWebApiIntegrationTestStartup<StartUp_AppInsights>(new StartUp_AppInsights(configuration))
  {
  }
}
