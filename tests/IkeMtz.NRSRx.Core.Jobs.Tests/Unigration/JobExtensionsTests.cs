using IkeMtz.Samples.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Unigration
{
  [TestClass]
  public class JobExtensionsTests
  {
    [TestMethod]
    public void SetupSplunkTest()
    {
      var job = new Program();
      var jobHost = job.SetupHost();
      var logger = jobHost.Services.GetService<ILogger<JobExtensionsTests>>();
      Assert.IsNotNull(logger);
    }
  }
}
