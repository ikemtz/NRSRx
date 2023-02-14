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
    /// </summary>
    public virtual bool EnablePendingMsgProcessing { get; set; } = false;

    /// <summary>
    /// Setting this to true will auto delete Idle Consumers
    /// </summary>
    public virtual bool AutoDeleteIdleConsumers { get; set; } = false;

    protected MessageFunction(ILogger<TMessageFunction> logger, RedisStreamSubscriber<TEntity, TEvent, TIdentityType> subscriber)
    {
      Logger = logger;
      Subscriber = subscriber;
    }

    public async Task<bool> RunAsync()
    {
      await LogStreamHealthInformationAsync();
      await ProcessStreamsAsync("new", Subscriber.GetMessagesAsync);

      if (EnablePendingMsgProcessing)
      {
        await ProcessStreamsAsync("pending", Subscriber.GetPendingMessagesAsync);
      }
      if (AutoDeleteIdleConsumers)
      {
        await Subscriber.DeleteIdleConsumersAsync();
      }
      return true;
    }

    public async Task ProcessStreamsAsync(string messageType, Func<int?, Task<IEnumerable<(RedisValue Id, TEntity Entity)>>> getMessageFunction)
    {
      Logger.LogInformation("Pulling {MessageBufferCount} {messageType} messages from queue.", MessageBufferCount, messageType);
      var messages = await getMessageFunction(MessageBufferCount);
      var messageCount = messages.Count();
      Logger.LogInformation("Received {messageCount} {messageType} messages from queue.", messageCount, messageType);
      foreach (var (redisId, entity) in messages)
      {
        var id = entity.Id;
        try
        {
          Logger.LogInformation("Handling {messageType} message with entity id: {id} ", messageType, id);
          await HandleMessageAsync(entity);
          Logger.LogInformation("Handled {messageType} message with entity id: {id} ", messageType, id);
          await Subscriber.AcknowledgeMessageAsync(redisId);
        }
        catch (Exception x)
        {
          Logger.LogError(x, "An error while handling {messageType} message with entity id: {id} ", messageType, id);
          throw;
        }
      }
    }

    public async Task LogStreamHealthInformationAsync()
    {
      var result = await Subscriber.GetStreamInfoAsync();
      if (result != null) // This can happen in mocked scenarios
      {
        Logger.LogInformation("Stream Message Count: {MessageCount}.", result.MessageCount);
        Logger.LogInformation("Stream Acknowledged Message Count: {AckMessageCount}.", result.AckMessageCount);
        Logger.LogInformation("Stream ConsumerGroup Count: {SubscriberCount}.", result.SubscriberCount);
        Logger.LogInformation("Stream Pending Message Count: {DeadLetterCount}.", result.DeadLetterCount);
      }
      else
      {
        Logger.LogWarning("Stream Info is not available.");
      }
    }

    public abstract Task HandleMessageAsync(TEntity entity);
  }
}
