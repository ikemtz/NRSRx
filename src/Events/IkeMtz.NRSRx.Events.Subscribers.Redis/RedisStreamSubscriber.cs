using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Abstraction.Redis;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Subscribers.Redis
{
  [ExcludeFromCodeCoverage]
  public class RedisStreamSubscriber<TEntity, TEvent> :
    RedisStreamSubscriber<TEntity, TEvent, Guid>
    where TEntity : IIdentifiable<Guid>
    where TEvent : EventType, new()
  {
    public RedisStreamSubscriber(IConnectionMultiplexer connection) : base(connection)
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

   public RedisValue? ConsumerName { get; set; }
    public string ConsumerGroupName { get; set; }
    public bool Subscribed { get; private set; }
    public event MessageRecievedEventHandler OnMessageReceived;
    public RedisStreamSubscriber(IConnectionMultiplexer connection) : base(connection)
    {
    }

    public virtual bool Init(string streamPosition = "$")
    {
      ConsumerName ??= Guid.NewGuid().ToString("N");
      ConsumerGroupName ??= $"cg{StreamKey}-{GetType().Assembly.GetName()}";
      try
      {
        return Database.StreamCreateConsumerGroup(StreamKey, ConsumerGroupName, streamPosition, true);
      }
      catch (RedisServerException x) when (x.Message.Contains("already exists"))
      {
        //We want to ignore this error
        return false;
      }
    }

    public virtual async Task<IEnumerable<(RedisValue Id, TEntity Entity)>> GetMessagesAsync(int messageCount = 1)
    {
      if (ConsumerName == null)
      {
        throw new NullReferenceException($"{nameof(ConsumerName)} is null, you probably forgot to call Init().");
      }
      var data = await Database.StreamReadGroupAsync(StreamKey, ConsumerGroupName, ConsumerName.Value, count: messageCount);
      return data.SelectMany(t => t.Values.Select(v => (t.Id, MessageCoder.JsonDecode<TEntity>(Convert.FromBase64String(v.Value)))));
    }

    public virtual async Task<IEnumerable<(RedisValue Id, TEntity Entity)>> GetPendingMessagesAsync(int messageCount = 1, int messageRetryCount = 3)
    {
      if (ConsumerName == null)
      {
        throw new NullReferenceException($"{nameof(ConsumerName)} is null, you probably forgot to call Init().");
      }
      var pendingMessageInfo = await GetConsumersWithPendingMessagesAsync();
      var messageList = new List<(RedisValue Id, TEntity Entity)>();
      foreach (var consumer in pendingMessageInfo.Where(t => t.PendingMessageCount > 0))
      {
        if (messageList.Count < messageCount)
        {
          var pendingMessages = await Database.StreamPendingMessagesAsync(StreamKey, ConsumerGroupName, messageCount, consumer.ConsumerName);
          var messageIds = pendingMessages.Where(t => t.DeliveryCount <= messageRetryCount).Select(t => t.MessageId).ToArray();
          if (messageIds.Any())
          {
            var data = await Database.StreamClaimAsync(StreamKey, ConsumerGroupName, ConsumerName.Value, 10000, messageIds);
            messageList.AddRange(data.SelectMany(t => t.Values.Select(v => (t.Id, MessageCoder.JsonDecode<TEntity>(Convert.FromBase64String(v.Value))))));
          }
        }
        else
        {
          break;
        }
      }
      return messageList;
    }
    public virtual async Task<IEnumerable<(string ConsumerName, int PendingMessageCount)>> GetConsumersWithPendingMessagesAsync()
    {
      var result = await Database.StreamPendingAsync(StreamKey, ConsumerGroupName, CommandFlags.None);
      return result.Consumers?.Select(t => (ConsumerName: t.Name.ToString(), t.PendingMessageCount));
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
