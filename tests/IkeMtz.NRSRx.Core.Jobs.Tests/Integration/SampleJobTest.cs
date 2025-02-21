using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Jobs;
using IkeMtz.Samples.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
      var dbContext = program.JobHost.Services.GetRequiredService<DatabaseContext>();
      Assert.IsTrue(await dbContext.Courses.AnyAsync());
      Assert.IsTrue(await dbContext.Schools.AnyAsync());
    }
  }
}
