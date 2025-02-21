using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Jobs.Tests.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Unigration
{
  [TestClass]
  public class SampleJobTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task SampleJobTest()
    {
      //arange
      var program = new UnigrationProgram(new Program(), TestContext);
      program.ExecuteOnContext<DatabaseContext>(x => x.Courses.Add(Factories.CourseFactory()));

      //act
      await program.RunAsync();

      //assert
      var dbContext = program.JobHost.Services.GetService<DatabaseContext>();
      Assert.IsTrue(await dbContext.Courses.AnyAsync());
      Assert.IsFalse(await dbContext.Schools.AnyAsync());
    }
  }
}
