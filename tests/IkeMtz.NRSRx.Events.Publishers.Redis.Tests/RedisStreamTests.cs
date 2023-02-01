using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Publishers.Redis.Tests
{
  [TestClass]
  public class RedisStreamTests
  {
    [TestMethod]
    [TestCategory("Integration")]
    public async Task ValidateRedisPublishAsync()
    {
      var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent, Guid>(connectionMultiplexer);
      var original = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      await publisher.PublishAsync(new SampleMessage());
      var result = await publisher.Database.StreamInfoAsync(publisher.StreamKey);
      Assert.AreEqual(original.Length + 1, result.Length);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateRedisMoqPublishAsync()
    {
      var (Connection, Database) = MockRedisStreamFactory.CreateMockConnection();
      var publisher = new RedisStreamPublisher<SampleMessage, CreateEvent, Guid>(Connection.Object);
      var msg = new SampleMessage();
      await publisher.PublishAsync(msg);
      Database
        .Verify(t => t.StreamAddAsync(publisher.StreamKey, It.Is<RedisValue>(x => x.StartsWith(msg.Id.ToString())), It.IsAny<RedisValue>(), null, null, false, CommandFlags.None), Times.Once);
    }
  }
}
