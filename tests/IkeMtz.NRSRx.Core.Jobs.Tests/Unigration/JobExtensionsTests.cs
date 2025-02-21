using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Logging.Splunk;
using IkeMtz.Samples.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.Jobs.Tests.Unigration
{
  [TestClass]
  public class JobExtensionsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public void SetupSplunkTest()
    {
      var job = new Program();
      var jobHost = job.SetupHost();
      var logger = jobHost.Services.GetService<ILogger<JobExtensionsTests>>();
      var seriLogger = SplunkExtensions.ConfigureSplunkLogger(job.Configuration);
      Assert.IsNotNull(logger);
      Assert.IsNotNull(seriLogger);
      Assert.IsNotNull(SplunkExtensions.ClientHandler);
    }
  }
}
