using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.Samples.Events.Redis.Publishers;
using IkeMtz.Samples.Models.V1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.Samples.Events.Redis.Tests.Unigration
{
  [TestClass]
  public class CoursePublisherTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void TestCreatePublisher()
    {
      var (Connection, _) = MockRedisStreamFactory<Course, CreatedEvent>.CreateConnection();
      var publisher = new CourseCreatedPublisher(Connection.Object);
      Assert.AreEqual(Connection.Object.GetDatabase(), publisher.Database);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void TestUpdatePublisher()
    {
      var (Connection, _) = MockRedisStreamFactory<Course, UpdatedEvent>.CreateConnection();
      var publisher = new CourseUpdatedPublisher(Connection.Object);
      Assert.AreEqual(Connection.Object.GetDatabase(), publisher.Database);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void TestDeletedPublisher()
    {
      var (Connection, _) = MockRedisStreamFactory<Course, DeletedEvent>.CreateConnection();
      var publisher = new CourseDeletedPublisher(Connection.Object);
      Assert.AreEqual(Connection.Object.GetDatabase(), publisher.Database);
    }
  }
}
