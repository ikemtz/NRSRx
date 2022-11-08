using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Jobs;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Integration
{
  public class IntegrationProgram : CoreJobIntegrationTestProgram<Program>
  {
    public IntegrationProgram(Program program, TestContext testContext) : base(program, testContext)
    {
    }
  }
}
