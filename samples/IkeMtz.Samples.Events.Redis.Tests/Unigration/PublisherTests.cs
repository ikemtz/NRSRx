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
    public void TestCreatePublisher()
    {
      var mockConnection = MockRedisStreamPublisherFactory<Item, CreatedEvent>.CreateConnection();
      var publisher = new ItemCreatedPublisher(mockConnection.Object);
      Assert.AreEqual(mockConnection.Object.GetDatabase(), publisher.Database);
    }
    [TestMethod]
    public void TestUpdatePublisher()
    {
      var mockConnection = MockRedisStreamPublisherFactory<Item, UpdatedEvent>.CreateConnection();
      var publisher = new ItemUpdatedPublisher(mockConnection.Object);
      Assert.AreEqual(mockConnection.Object.GetDatabase(), publisher.Database);
    }
    [TestMethod]
    public void TestDeletedPublisher()
    {
      var mockConnection = MockRedisStreamPublisherFactory<Item, DeletedEvent>.CreateConnection();
      var publisher = new ItemDeletedPublisher(mockConnection.Object);
      Assert.AreEqual(mockConnection.Object.GetDatabase(), publisher.Database);
    }
  }
}
