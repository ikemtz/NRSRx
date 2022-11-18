using IkeMtz.NRSRx.Core.Unigration.Jobs;
using IkeMtz.Samples.Redis.Jobs;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Redis.Integration
{
  public class IntegrationProgram : CoreMessagingIntegrationTestJob<Program>
  {
    public IntegrationProgram(Program program, TestContext testContext) : base(program, testContext)
    {
    }
  }
}
