using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.EntityFrameworkCore;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Integration
{
  [TestClass]
  public class SampleJobTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("SqlIntegration")]
    public async Task SampleJobTest()
    {
      //arange
      var program = new IntegrationProgram(new Program(), TestContext);
      program.ExecuteOnContext<DatabaseContext>(x => x.Courses.Add(Factories.CourseFactory()));

      //act
      await program.RunAsync();

      //assert
      program.ExecuteOnContext<DatabaseContext>(async x =>
      {
        var schoolCount = await x.Schools.CountAsync();
        Assert.IsTrue(1 <= schoolCount);
        var courseCount = await x.Courses.CountAsync();
        Assert.IsTrue(1 <= courseCount);
      });
    }
  }
}
