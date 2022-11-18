using IkeMtz.NRSRx.Core.Jobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class CoreIntegrationTestJob<TProgram> : CoreUnigrationTestJob<TProgram>
        where TProgram : class, IJob
  {
    public CoreIntegrationTestJob(TProgram program, TestContext testContext) : base(program, testContext)
    {
    }
  }
}
