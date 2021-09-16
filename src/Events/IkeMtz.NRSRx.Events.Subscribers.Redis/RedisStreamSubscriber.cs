using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Abstraction.Redis;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Subscribers.Redis
{
  public class RedisStreamSubscriber<TEntity, TEvent> :
    RedisStreamSubscriber<TEntity, TEvent, Guid>
    where TEntity : IIdentifiable<Guid>
    where TEvent : EventType, new()
  {
    public RedisStreamSubscriber(IConnectionMultiplexer connection, string streamPosition = "$") : base(connection, streamPosition)
    {
    }
  }

  public class RedisStreamSubscriber<TEntity, TEvent, TIdentityType> :
    RedisStreamCore<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {

    public delegate void MessageRecievedEventHandler(TEntity entity);

    public Guid InstanceId { get; }
    public RedisValue ConsumerName { get; }
    public string ConsumerGroupName { get; }
    public bool Subscribed { get; private set; }
    public event MessageRecievedEventHandler OnMessageReceived;
    public RedisStreamSubscriber(IConnectionMultiplexer connection, string streamPosition = "$") : base(connection)
    {
      InstanceId = Guid.NewGuid();
      ConsumerName = InstanceId.ToString("N");
      ConsumerGroupName = $"cg{StreamKey}";
      try
      {
        Database.StreamCreateConsumerGroup(StreamKey, ConsumerGroupName, streamPosition);
      }
      catch (RedisServerException x) when (x.Message.Contains("already exists"))
      {
        //We want to ignore this error
      }
    }

    public virtual async Task<IEnumerable<(RedisValue Id, TEntity Entity)>> GetMessagesAsync(int messageCount = 1)
    {
      var data = await Database.StreamReadGroupAsync(StreamKey, ConsumerGroupName, ConsumerName, count: messageCount);
      return data.SelectMany(t => t.Values.Select(v => (t.Id, MessageCoder.JsonDecode<TEntity>(Convert.FromBase64String(v.Value)))));
    }

    public virtual async Task<IEnumerable<(RedisValue Id, TEntity Entity)>> GetPendingMessagesAsync(int messageCount = 1)
    {
      var pendingMessages = await Database.StreamPendingMessagesAsync(StreamKey, ConsumerGroupName, messageCount, ConsumerName);
      var messageIds = pendingMessages.Select(t => t.MessageId).ToArray();
      if (messageIds.Any())
      {
        var data = await Database.StreamClaimAsync(StreamKey, ConsumerGroupName, ConsumerName, 10000, messageIds);
        return data.SelectMany(t => t.Values.Select(v => (t.Id, MessageCoder.JsonDecode<TEntity>(Convert.FromBase64String(v.Value)))));
      }
      return Array.Empty<(RedisValue Id, TEntity Entity)>();
    }

    public virtual Task<long> AcknowledgeMessageAsync(RedisValue redisValue)
    {
      return Database.StreamAcknowledgeAsync(StreamKey, ConsumerGroupName, redisValue);
    }

    public async Task Subscribe(int pollFrequency = 60000)
    {
      if (OnMessageReceived == null)
      {
        throw new InvalidProgramException("On Message Recieved event must be handled before subscribing");
      }
      while (true)
      {
        foreach (var (id, entity) in await GetMessagesAsync(10))
        {
          OnMessageReceived.Invoke(entity);
        }
        await Task.Delay(pollFrequency);
      }
    }
  }
}
