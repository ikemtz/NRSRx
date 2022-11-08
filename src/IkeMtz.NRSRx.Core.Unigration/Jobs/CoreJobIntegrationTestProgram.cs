using IkeMtz.NRSRx.Core.Jobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreJobIntegrationTestProgram<TProgram> : CoreJobUnigrationTestProgram<TProgram>
        where TProgram : class, IJob
  {
    public CoreJobIntegrationTestProgram(TProgram program, TestContext testContext) : base(program, testContext)
    {
    }
  }
}
