using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.OData.Tests;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Jobs;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Unigration
{
  [TestClass]
  public class SampleJobTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task SampleJobTest()
    {
      var program = new UnigrationProgram(new Program(), TestContext);
      program.ExecuteOnContext<DatabaseContext>(x => x.Schools.Add(Factories.SchoolFactory()));
      program.ExecuteOnContext<DatabaseContext>(x => x.Courses.Add(Factories.CourseFactory()));
      await program.RunAsync();
    }
  }
}
