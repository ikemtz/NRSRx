using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Jobs;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Unigration
{
  internal class UnigrationProgram : CoreJobUnigrationTestProgram<Program>
  {
    public UnigrationProgram(Program program, TestContext testContext) : base(program, testContext)
    {
    }
  }
}
