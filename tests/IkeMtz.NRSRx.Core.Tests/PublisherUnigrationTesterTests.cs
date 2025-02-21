using System;
using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.WebApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class PublisherUnigrationTesterTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidatePublisherUnigrationTypedIdTester()
    {
      var tester = new PublisherUnigrationTester<MyIntModel, Message, int>();
      await tester.CreatePublisher.Object.PublishAsync(new MyIntModel { Id = 1 });
      await tester.CreatedPublisher.Object.PublishAsync(new MyIntModel { Id = 2 });
      await tester.UpdatedPublisher.Object.PublishAsync(new MyIntModel { Id = 3 });
      await tester.DeletedPublisher.Object.PublishAsync(new MyIntModel { Id = 4 });
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

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task ValidatePublisherUnigrationGuidIdTester()
    {
      var tester = new PublisherUnigrationTester<MyGuidModel, Message>();
      await tester.GuidCreatePublisher.Object.PublishAsync(new MyGuidModel { Id = Guid.NewGuid() });
      await tester.GuidCreatedPublisher.Object.PublishAsync(new MyGuidModel { Id = Guid.NewGuid() });
      await tester.GuidUpdatedPublisher.Object.PublishAsync(new MyGuidModel { Id = Guid.NewGuid() });
      await tester.GuidDeletedPublisher.Object.PublishAsync(new MyGuidModel { Id = Guid.NewGuid() });
      Assert.AreEqual(4,
       tester.CreateList.Count +
       tester.CreatedList.Count +
       tester.UpdatedList.Count +
       tester.DeletedList.Count);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void ValidateDependencyRegistration()
    {
      var serviceCollection = new ServiceCollection();
      var tester = new PublisherUnigrationTester<MyGuidModel, Message>();
      tester.RegisterDependencies(serviceCollection);
      Assert.AreEqual(8, serviceCollection.Count);
    }
  }
}
