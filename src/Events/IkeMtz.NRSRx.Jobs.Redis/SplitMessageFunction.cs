using IkeMtz.NRSRx.Core.Jobs;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Abstraction;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace IkeMtz.NRSRx.Jobs.Redis
{
  public abstract class SplitMessageFunction<TSplitMessageFunction, TEntity, TEvent>
    : MessageFunction<TSplitMessageFunction, SplitMessage<TEntity>, TEvent, Guid>
      where TSplitMessageFunction : IMessageFunction
      where TEntity : class, IIdentifiable<Guid>
      where TEvent : EventType, new()
  {
    protected SplitMessageFunction(ILogger<TSplitMessageFunction> logger, RedisStreamSubscriber<SplitMessage<TEntity>, TEvent> subscriber)
      : base(logger, subscriber)
    { }

    public override async Task ProcessStreamsAsync(string messageType, Func<int?,
      Task<IEnumerable<(RedisValue Id, SplitMessage<TEntity> Entity)>>> getMessageFunction)
    {
      Logger.LogInformation("Pulling {MessageBufferCount} {messageType} messages from queue.", MessageBufferCount, messageType);
      var messages = await getMessageFunction(MessageBufferCount);
      var processedMessageCount = 0;
      var messageCount = 0;
      while ((messageCount = messages.Count()) > 0)
      {
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
            await NotifySplitProgress(entity, true);
            processedMessageCount++;
          }
          catch (Exception x)
          {
            Logger.LogError(x, "An error while handling {messageType} message with entity id: {id} ", messageType, id);
            await NotifySplitProgress(entity, false);
          }
        }
      }
      Logger.LogInformation("Processed {processedMessageCount} {messageType} messages from queue.", processedMessageCount, messageType);
    }

    public virtual Task NotifySplitProgress(SplitMessage<TEntity> entity, bool isSuccess)
    {
      return Task.CompletedTask;
    }
  }

}
