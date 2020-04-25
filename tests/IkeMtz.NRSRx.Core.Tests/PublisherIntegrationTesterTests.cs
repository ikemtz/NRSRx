using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.WebApi;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class PublisherIntegrationTesterTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidatePublisherIntegrationTester()
    {
      var tester = new PublisherIntegrationTester<MyModel, Message, int>();
      await tester.CreatePublisher.Object.PublishAsync(new MyModel { Id = 1 });
      await tester.CreatedPublisher.Object.PublishAsync(new MyModel { Id = 2 });
      await tester.UpdatedPublisher.Object.PublishAsync(new MyModel { Id = 3 });
      await tester.DeletedPublisher.Object.PublishAsync(new MyModel { Id = 4 });
      Assert.AreEqual(10,
        tester.CreateList.Sum(t => t.Id) +
        tester.CreatedList.Sum(t => t.Id) +
        tester.UpdatedList.Sum(t => t.Id) +
        tester.DeletedList.Sum(t => t.Id));
      Assert.AreEqual(4,
       tester.CreateList.Count +
       tester.CreatedList.Count +
       tester.UpdatedList.Count +
       tester.DeletedList.Count);
    }
  }
}
