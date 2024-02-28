using IkeMtz.NRSRx.Core.Jobs;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Jobs.Redis
{
  public abstract class MessageFunction<TMessageFunction, TEntity, TEvent> : MessageFunction<TMessageFunction, TEntity, TEvent, Guid>
    where TMessageFunction : IMessageFunction
    where TEntity : class, IIdentifiable<Guid>
    where TEvent : EventType, new()
  {
    protected MessageFunction(ILogger<TMessageFunction> logger, RedisStreamSubscriber<TEntity, TEvent, Guid> subscriber) : base(logger, subscriber)
    {
    }
  }

  public abstract class MessageFunction<TMessageFunction, TEntity, TEvent, TIdentityType> : IMessageFunction
    where TMessageFunction : IMessageFunction
    where TIdentityType : IComparable
    where TEntity : class, IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    public ILogger<TMessageFunction> Logger { get; }
    public virtual RedisStreamSubscriber<TEntity, TEvent, TIdentityType> Subscriber { get; set; }
    public virtual int MessageBufferCount { get; set; } = 5;

    /// <summary>
    /// Setting this to true will setup subscribers to auto claim messages from Idle Consumers
    /// Default value = true
    /// </summary>
    public virtual bool EnablePendingMsgProcessing { get; set; } = true;

    /// <summary>
    /// The higher priorty functions get run first (ordered by descending). 
    /// </summary>
    public virtual int? SequencePriority { get; set; } = null;

    protected MessageFunction(ILogger<TMessageFunction> logger, RedisStreamSubscriber<TEntity, TEvent, TIdentityType> subscriber)
    {
      Logger = logger;
      Subscriber = subscriber;
    }

    public virtual async Task<bool> RunAsync()
    {
      await LogStreamHealthInformationAsync();
      await ProcessStreamsAsync("new", Subscriber.GetMessagesAsync);

      if (EnablePendingMsgProcessing)
      {
        await ProcessStreamsAsync("pending", Subscriber.GetPendingMessagesAsync);
      }
      await Subscriber.DeleteIdleConsumersAsync();
      Subscriber.ConsumerName = Guid.NewGuid().ToString("N");
      return true;
    }

    public virtual async Task ProcessStreamsAsync(string messageType, Func<int?, Task<IEnumerable<(RedisValue Id, TEntity Entity)>>> getMessageFunction)
    {
      Logger.LogInformation("Pulling {MessageBufferCount} {messageType} messages from queue.", MessageBufferCount, messageType);
      var messages = await getMessageFunction(MessageBufferCount);
      var messageCount = messages.Count();
      Logger.LogInformation("Received {messageCount} {messageType} messages from queue.", messageCount, messageType);
      var processedMessageCount = 0;
      foreach (var (redisId, entity) in messages)
      {
        var id = entity.Id;
        try
        {
          Logger.LogInformation("Handling {messageType} message with entity id: {id} ", messageType, id);
          await HandleMessageAsync(entity);
          Logger.LogInformation("Handled {messageType} message with entity id: {id} ", messageType, id);
          await Subscriber.AcknowledgeMessageAsync(redisId);
          processedMessageCount++;
        }
        catch (Exception x)
        {
          Logger.LogError(x, "An error while handling {messageType} message with entity id: {id} ", messageType, id);

        }
      }
      Logger.LogInformation("Processed {processedMessageCount} {messageType} messages from queue.", processedMessageCount, messageType);
    }

    public virtual async Task LogStreamHealthInformationAsync()
    {
      var result = await Subscriber.GetStreamInfoAsync();
      if (result != null) // This can happen in mocked scenarios
      {
        Logger.LogInformation("Consumer Group {ConsumerGroupName}, Message Count: {MessageCount}.", result.StreamKey, result.MessageCount);
        Logger.LogInformation("Consumer Group {ConsumerGroupName}, Acknowledged Message Count: {AckMessageCount}.", result.StreamKey, result.AckMessageCount);
        Logger.LogInformation("Consumer Group {ConsumerGroupName}, Consumer Count: {SubscriberCount}.", result.StreamKey, result.SubscriberCount);
        Logger.LogInformation("Consumer Group {ConsumerGroupName}, Pending Count: {PendingMsgCount}.", result.StreamKey, result.PendingMsgCount);
        Logger.LogInformation("Consumer Group {ConsumerGroupName}, Dead Letter Message Count: {DeadLetterCount}.", result.StreamKey, result.DeadLetterMsgCount);
      }
      else
      {
        Logger.LogWarning("Stream Info is not available.");
      }
    }

    public abstract Task HandleMessageAsync(TEntity entity);
  }
}
