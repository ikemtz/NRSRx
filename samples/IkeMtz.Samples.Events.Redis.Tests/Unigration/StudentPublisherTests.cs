using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.Samples.Events.Redis.Publishers;
using IkeMtz.Samples.Models.V1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.Samples.Events.Redis.Tests.Unigration
{
  [TestClass]
  public class StudentPublisherTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void TestCreatePublisher()
    {
      var (Connection, _) = MockRedisStreamFactory<Student, CreatedEvent>.CreateConnection();
      var publisher = new StudentCreatedPublisher(Connection.Object);
      Assert.AreEqual(Connection.Object.GetDatabase(), publisher.Database);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void TestUpdatePublisher()
    {
      var (Connection, _) = MockRedisStreamFactory<Student, UpdatedEvent>.CreateConnection();
      var publisher = new StudentUpdatedPublisher(Connection.Object);
      Assert.AreEqual(Connection.Object.GetDatabase(), publisher.Database);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public void TestDeletedPublisher()
    {
      var (Connection, _) = MockRedisStreamFactory<Student, DeletedEvent>.CreateConnection();
      var publisher = new StudentDeletedPublisher(Connection.Object);
      Assert.AreEqual(Connection.Object.GetDatabase(), publisher.Database);
    }
  }
}
