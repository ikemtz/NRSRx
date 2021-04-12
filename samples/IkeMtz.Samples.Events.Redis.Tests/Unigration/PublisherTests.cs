using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.Samples.Events.Redis.Publishers;
using IkeMtz.Samples.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.Samples.Events.Redis.Tests.Unigration
{
  [TestClass]
  public class PublisherTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void TestCreatePublisher()
    {
      var (Connection, _) = MockRedisStreamFactory<Item, CreatedEvent>.CreateConnection();
      var publisher = new ItemCreatedPublisher(Connection.Object);
      Assert.AreEqual(Connection.Object.GetDatabase(), publisher.Database);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void TestUpdatePublisher()
    {
      var (Connection, _) = MockRedisStreamFactory<Item, UpdatedEvent>.CreateConnection();
      var publisher = new ItemUpdatedPublisher(Connection.Object);
      Assert.AreEqual(Connection.Object.GetDatabase(), publisher.Database);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void TestDeletedPublisher()
    {
      var (Connection, _) = MockRedisStreamFactory<Item, DeletedEvent>.CreateConnection();
      var publisher = new ItemDeletedPublisher(Connection.Object);
      Assert.AreEqual(Connection.Object.GetDatabase(), publisher.Database);
    }
  }
}
