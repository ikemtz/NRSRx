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
  /// <summary>
  /// Redis stream subscriber for handling events with a GUID identifier.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  [ExcludeFromCodeCoverage]
  public class RedisStreamSubscriber<TEntity, TEvent> :
      RedisStreamSubscriber<TEntity, TEvent, Guid>
      where TEntity : class, IIdentifiable<Guid>
      where TEvent : EventType, new()
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RedisStreamSubscriber{TEntity, TEvent}"/> class.
    /// </summary>
    /// <param name="connection">The Redis connection multiplexer.</param>
    /// <param name="options">The subscriber options.</param>
    public RedisStreamSubscriber(IConnectionMultiplexer connection, RedisSubscriberOptions? options = null) : base(connection, options)
    {
    }
  }

  /// <summary>
  /// Redis stream subscriber for handling events.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  /// <typeparam name="TIdentityType">The type of the identity.</typeparam>
  public class RedisStreamSubscriber<TEntity, TEvent, TIdentityType> :
    RedisStreamCore<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : class, IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    /// <summary>
    /// Delegate for handling received messages.
    /// </summary>
    /// <param name="entity">The received entity.</param>
    public delegate void MessageRecievedEventHandler(TEntity entity);

    /// <summary>
    /// Gets or sets the consumer name.
    /// </summary>
    public RedisValue? ConsumerName { get; set; }

    /// <summary>
    /// Gets the consumer group name.
    /// </summary>
    public string ConsumerGroupName { get; private set; }

    /// <summary>
    /// Gets the dead consumer name.
    /// </summary>
    public string DeadConsumerName { get; } = "dead-letter";

    /// <summary>
    /// Gets or sets the consumer group acknowledgment counter key.
    /// </summary>
    public string ConsumerGroupAckCounterKey { get; set; }

    /// <summary>
    /// Gets a value indicating whether the subscriber is subscribed.
    /// </summary>
    public bool Subscribed { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the subscriber is initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets the subscriber options.
    /// </summary>
    public RedisSubscriberOptions Options { get; }

    /// <summary>
    /// Occurs when a message is received.
    /// </summary>
    public event MessageRecievedEventHandler OnMessageReceived;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisStreamSubscriber{TEntity, TEvent, TIdentityType}"/> class.
    /// </summary>
    /// <param name="connection">The Redis connection multiplexer.</param>
    /// <param name="options">The subscriber options.</param>
    public RedisStreamSubscriber(IConnectionMultiplexer connection, RedisSubscriberOptions? options = null) : base(connection)
    {
      Options = options ?? new RedisSubscriberOptions();
    }

    /// <summary>
    /// Initializes the subscriber.
    /// </summary>
    /// <returns>True if initialization is successful; otherwise, false.</returns>
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
        ConsumerGroupAckCounterKey ??= $"{ConsumerGroupName}-AckCnt";

        try
        {
          _ = Database.StringIncrementAsync(ConsumerGroupAckCounterKey, 0);
          var result = Database.StreamCreateConsumerGroup(StreamKey, ConsumerGroupName, Options.StartPosition, true);
          IsInitialized = true;
        }
        catch (RedisServerException x) when (x.Message.Contains("already exists"))
        {
          IsInitialized = true;
          // We want to ignore this error
          return false;
        }
      }
      return IsInitialized;
    }

    /// <summary>
    /// Gets the stream information asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the message queue information.</returns>
    public virtual async Task<MessageQueueInfo> GetStreamInfoAsync()
    {
      ValidateInit();
      var info = await Database.StreamInfoAsync(StreamKey);
      var ackMsgCount = Convert.ToInt32(await Database.StringGetAsync(ConsumerGroupAckCounterKey));
      var pendingInfo = await GetConsumerInfoAsync();
      return new MessageQueueInfo
      {
        StreamKey = StreamKey,
        MsgCount = info.Length,
        AcknowledgedMsgCount = ackMsgCount,
        SubscriberCount = pendingInfo.Count(),
        PendingMsgCount = pendingInfo.Where(t => t.Name != this.DeadConsumerName).Sum(t => t.PendingMsgCount),
        DeadLetterMsgCount = pendingInfo.FirstOrDefault(t => t.Name == this.DeadConsumerName)?.PendingMsgCount ?? 0,
      };
    }

    /// <summary>
    /// Gets the messages asynchronously.
    /// </summary>
    /// <param name="messageCount">The number of messages to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the messages.</returns>
    public virtual async Task<IEnumerable<(RedisValue Id, TEntity Entity)>> GetMessagesAsync(int? messageCount = null)
    {
      ValidateInit();
      var data = await Database.StreamReadGroupAsync(StreamKey, ConsumerGroupName, ConsumerName.GetValueOrDefault(), count: messageCount ?? Options.MessagesPerBatchCount);
      return data.SelectMany(t => t.Values.Select(v => (t.Id, JsonConvert.DeserializeObject<TEntity>(v.Value))));
    }

    /// <summary>
    /// Deletes idle consumers and returns the deleted count.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of deleted consumers.</returns>
    public virtual async Task<int> DeleteIdleConsumersAsync()
    {
      var data = await Database.StreamConsumerInfoAsync(StreamKey, ConsumerGroupName);
      var idleConsumers = data
        .Where(t => t.Name != DeadConsumerName)
        .Where(t => t.Name != ConsumerName)
        .Where(t => t.IdleTimeInMilliseconds > Options.IdleTimeSpanInMilliseconds && t.PendingMessageCount == 0).ToList();
      foreach (var consumer in idleConsumers)
      {
        await Database.StreamDeleteConsumerAsync(StreamKey, ConsumerGroupName, consumer.Name);
      }
      return idleConsumers.Count;
    }

    /// <summary>
    /// Gets the pending messages asynchronously.
    /// </summary>
    /// <param name="messageCount">The number of messages to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the pending messages.</returns>
    public virtual async Task<IEnumerable<(RedisValue Id, TEntity Entity)>> GetPendingMessagesAsync(int? messageCount = null)
    {
      ValidateInit();
      messageCount ??= Options.PendingMessagesPerBatchCount;
      var pendingConsumerNames = new List<string> { ConsumerName.GetValueOrDefault() };
      pendingConsumerNames.AddRange((await GetIdleConsumersWithPendingMsgsAsync()).Select(t => t.Name));
      var messageList = new List<(RedisValue Id, TEntity Entity)>();
      foreach (var consumer in pendingConsumerNames.Distinct())
      {
        if (messageList.Count < messageCount)
        {
          var pendingMessages = await Database.StreamPendingMessagesAsync(StreamKey, ConsumerGroupName, 1000, consumer);
          var messageIds = pendingMessages.Where(t => t.DeliveryCount <= Options.MaxMessageProcessRetry).Select(t => t.MessageId).ToArray();
          if (messageIds.Length != 0)
          {
            var data = await Database.StreamClaimAsync(StreamKey, ConsumerGroupName, ConsumerName.GetValueOrDefault(), 10000, messageIds);
            messageList.AddRange(data.SelectMany(t => t.Values.Select(v => (t.Id, JsonConvert.DeserializeObject<TEntity>(v.Value)))));
          }
          var deadMessageIds = pendingMessages
            .Where(t => t.DeliveryCount > Options.MaxMessageProcessRetry)
            .Select(t => t.MessageId).ToArray();
          if (deadMessageIds.Length != 0)
          {
            var data = await Database.StreamClaimAsync(StreamKey, ConsumerGroupName, DeadConsumerName, 10000, deadMessageIds);
          }
        }
        else
        {
          break;
        }
      }
      return messageList;
    }

    /// <summary>
    /// Gets the consumer information asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the consumer information.</returns>
    public virtual async Task<IEnumerable<RedisStreamConsumerMetadata>> GetConsumerInfoAsync()
    {
      ValidateInit();
      var data = await Database.StreamConsumerInfoAsync(StreamKey, ConsumerGroupName);
      return data.Select(t => new RedisStreamConsumerMetadata { Name = t.Name, IdleTimeInMs = t.IdleTimeInMilliseconds, PendingMsgCount = t.PendingMessageCount });
    }

    /// <summary>
    /// Gets the idle consumers with pending messages asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the idle consumers with pending messages.</returns>
    public virtual async Task<IEnumerable<RedisStreamConsumerMetadata>> GetIdleConsumersWithPendingMsgsAsync()
    {
      var data = await GetConsumerInfoAsync();
      var idleConsumers = data
        .Where(t => t.Name != this.DeadConsumerName)
        .Where(t => t.IdleTimeInMs > Options.IdleTimeSpanInMilliseconds && t.PendingMsgCount > 0);
      return idleConsumers;
    }

    /// <summary>
    /// Acknowledges a message asynchronously.
    /// </summary>
    /// <param name="redisValue">The Redis value of the message to acknowledge.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the acknowledgment result.</returns>
    public virtual async Task<long> AcknowledgeMessageAsync(RedisValue redisValue)
    {
      var result = await Database.StreamAcknowledgeAsync(StreamKey, ConsumerGroupName, redisValue);
      _ = await Database.StringIncrementAsync(ConsumerGroupAckCounterKey, result);
      return result;
    }

    /// <summary>
    /// Subscribes to the stream and processes messages.
    /// </summary>
    /// <param name="pollFrequency">The frequency to poll for new messages, in milliseconds.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidProgramException">Thrown if the OnMessageReceived event is not handled before subscribing.</exception>
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

    /// <summary>
    /// Validates the initialization of the subscriber.
    /// </summary>
    private void ValidateInit()
    {
      if (!IsInitialized)
      {
        _ = Init();
      }
    }
  }
}
