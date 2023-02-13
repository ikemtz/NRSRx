using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction.Redis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Events.Subscribers.Redis
{
  [ExcludeFromCodeCoverage]
  public class RedisStreamSubscriber<TEntity, TEvent> :
    RedisStreamSubscriber<TEntity, TEvent, Guid>
    where TEntity : class, IIdentifiable<Guid>
    where TEvent : EventType, new()
  {
    public RedisStreamSubscriber(IConnectionMultiplexer connection, RedisSubscriberOptions? options = null) : base(connection, options)
    {
    }
  }

  public class RedisStreamSubscriber<TEntity, TEvent, TIdentityType> :
    RedisStreamCore<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : class, IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    public delegate void MessageRecievedEventHandler(TEntity entity);
    public RedisValue? ConsumerName { get; set; }
    public string ConsumerGroupName { get; private set; }
    public string ConsumerGroupCounterKey { get; set; }
    public bool Subscribed { get; private set; }
    public bool IsInitialized { get; private set; }
    public RedisSubscriberOptions Options { get; }

    public event MessageRecievedEventHandler OnMessageReceived;
    public RedisStreamSubscriber(IConnectionMultiplexer connection, RedisSubscriberOptions? options = null) : base(connection)
    {
      Options = options ?? new RedisSubscriberOptions();
    }

    public virtual bool Init()
    {
      if (!IsInitialized)
      {
        var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
        ConsumerName = Guid.NewGuid().ToString("N");
        ConsumerGroupName = Options.ConsumerGroupName ?? $"{StreamKey}:{assemblyName}";
        if (!ConsumerGroupName.StartsWith(StreamKey))
        {
          ConsumerGroupName = $"{StreamKey}:{ConsumerGroupName}";
        }
        ConsumerGroupCounterKey ??= $"{ConsumerGroupName}-AckCnt";

        try
        {
          _ = Database.StringIncrementAsync(ConsumerGroupCounterKey, 0);
          var result = Database.StreamCreateConsumerGroup(StreamKey, ConsumerGroupName, Options.StartPosition, true);
          IsInitialized = true;
        }
        catch (RedisServerException x) when (x.Message.Contains("already exists"))
        {
          IsInitialized = true;
          //We want to ignore this error
          return false;
        }
      }
      return IsInitialized;
    }

    public virtual async Task<MessageQueueInfo> GetStreamInfoAsync()
    {
      ValidateInit();
      var info = await Database.StreamInfoAsync(StreamKey);
      var ackMsgCount = Convert.ToInt32(await Database.StringGetAsync(ConsumerGroupCounterKey));
      var pendingInfo = await GetConsumerInfoAsync();
      return new MessageQueueInfo
      {
        MessageCount = info.Length,
        AckMessageCount = ackMsgCount,
        SubscriberCount = pendingInfo.Count(),
        DeadLetterCount = pendingInfo.Sum(t => t.PendingMsgCount),
      };
    }

    public virtual async Task<IEnumerable<(RedisValue Id, TEntity Entity)>> GetMessagesAsync(int messageCount = 1)
    {
      ValidateInit();
      var data = await Database.StreamReadGroupAsync(StreamKey, ConsumerGroupName, ConsumerName.GetValueOrDefault(), count: messageCount);
      return data.SelectMany(t => t.Values.Select(v => (t.Id, JsonConvert.DeserializeObject<TEntity>(v.Value))));
    }

    /// <summary>
    /// Will delete Idle consumers and return deleted count
    /// </summary>
    /// <param name="idleTimeInMilliSeconds"></param>
    /// <returns></returns>
    public virtual async Task<int> DeleteIdleConsumersAsync()
    {
      var data = await Database.StreamConsumerInfoAsync(StreamKey, ConsumerGroupName);
      var idleConsumers = data.Where(t => t.IdleTimeInMilliseconds > Options.IdleTimeSpanInMilliseconds && t.PendingMessageCount == 0).ToList();
      foreach (var consumer in idleConsumers)
      {
        await Database.StreamDeleteConsumerAsync(StreamKey, ConsumerGroupName, consumer.Name);
        await Database.KeyDeleteAsync(ConsumerGroupCounterKey);
      }
      return idleConsumers.Count;
    }

    public virtual async Task<IEnumerable<(RedisValue Id, TEntity Entity)>> GetPendingMessagesAsync(int messageCount = 1)
    {
      ValidateInit();
      var pendingConsumers = await GetIdleConsumersWithPendingMsgsAsync();
      var messageList = new List<(RedisValue Id, TEntity Entity)>();
      foreach (var consumer in pendingConsumers)
      {
        if (messageList.Count < messageCount)
        {
          var pendingMessages = await Database.StreamPendingMessagesAsync(StreamKey, ConsumerGroupName, messageCount, consumer.Name);
          var messageIds = pendingMessages.Where(t => t.DeliveryCount <= Options.MaxMessageProcessRetry).Select(t => t.MessageId).ToArray();
          if (messageIds.Any())
          {
            var data = await Database.StreamClaimAsync(StreamKey, ConsumerGroupName, ConsumerName.GetValueOrDefault(), 10000, messageIds);
            messageList.AddRange(data.SelectMany(t => t.Values.Select(v => (t.Id, JsonConvert.DeserializeObject<TEntity>(v.Value)))));
          }
        }
        else
        {
          break;
        }
      }
      return messageList;
    }

    public virtual async Task<IEnumerable<Consumer>> GetConsumerInfoAsync()
    {
      ValidateInit();
      var data = await Database.StreamConsumerInfoAsync(StreamKey, ConsumerGroupName);
      return data.Select(t => new Consumer { Name = t.Name, IdleTimeInMs = t.IdleTimeInMilliseconds, PendingMsgCount = t.PendingMessageCount });
    }

    public virtual async Task<IEnumerable<Consumer>> GetIdleConsumersWithPendingMsgsAsync()
    {
      var data = await GetConsumerInfoAsync();
      var idleConsumers = data.Where(t => t.IdleTimeInMs > Options.IdleTimeSpanInMilliseconds && t.PendingMsgCount > 0);
      return idleConsumers;
    }

    public virtual async Task<long> AcknowledgeMessageAsync(RedisValue redisValue)
    {
      var result = await Database.StreamAcknowledgeAsync(StreamKey, ConsumerGroupName, redisValue);
      _ = await Database.StringIncrementAsync(ConsumerGroupCounterKey, result);
      return result;
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

    private void ValidateInit()
    {
      if (!IsInitialized)
      {
        _ = Init();
      }
    }
  }
}
