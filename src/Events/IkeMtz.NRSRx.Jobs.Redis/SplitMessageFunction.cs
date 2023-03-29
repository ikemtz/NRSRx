using System.Diagnostics.CodeAnalysis;
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
    public bool AutoDeleteSplitProgressData { get; set; } = true;
    public const string PASS = "Passed";
    public const string FAIL = "Failed";
    protected SplitMessageFunction(ILogger<TSplitMessageFunction> logger,
      RedisStreamSubscriber<SplitMessage<TEntity>, TEvent> subscriber)
      : base(logger, subscriber)
    { }

    [ExcludeFromCodeCoverage]
    public override async Task ProcessStreamsAsync(string messageType, Func<int?,
      Task<IEnumerable<(RedisValue Id, SplitMessage<TEntity> Entity)>>> getMessageFunction)
    {
      IEnumerable<(RedisValue Id, SplitMessage<TEntity> Entity)> messages;
      do
      {
        messages = await getMessageFunction(MessageBufferCount);
        await ProcessStreamSplitBatchAsync(messageType, messages);
      }
      while (messages.Any());
    }

    public virtual async Task ProcessStreamSplitBatchAsync(string messageType, IEnumerable<(RedisValue Id, SplitMessage<TEntity> Entity)> messages)
    {
      Logger.LogInformation("Pulling {MessageBufferCount} {messageType} messages from queue.", MessageBufferCount, messageType);
      Logger.LogInformation("Received {messageCount} {messageType} messages from queue.", messages.Count(), messageType);
      int processedMessageCount = 0;
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
      Logger.LogInformation("Processed {processedMessageCount} {messageType} messages from queue.", processedMessageCount, messageType);
    }

    public virtual async Task<SplitMessageProgressUpdate> NotifySplitProgress(SplitMessage<TEntity> entity, bool isSuccess)
    {
      var passFail = isSuccess ? PASS : FAIL;
      var incrementKey = $"{Subscriber.StreamKey}:{entity.Id}";
      await Subscriber.Database.HashIncrementAsync(incrementKey, passFail, 1);
      var result = await Subscriber.Database.HashGetAllAsync(incrementKey);

      var progressUpdate = ConvertHashSet(result, entity.TaskCount);
      if (entity.TaskCount <= progressUpdate.Passed + progressUpdate.Failed)
      {
        if (AutoDeleteSplitProgressData)
        {
          await Subscriber.Database.KeyDeleteAsync(incrementKey);
        }
        await NotifySplitCompletion(entity);
      }
      return progressUpdate;
    }

    public virtual SplitMessageProgressUpdate ConvertHashSet(HashEntry[] result, int totalMessages)
    {
      var passVal = result.FirstOrDefault(t => t.Name == PASS);
      var failVal = result.FirstOrDefault(t => t.Name == FAIL);
      return new SplitMessageProgressUpdate()
      {
        Total = totalMessages,
        Passed = Convert.ToInt32(passVal.Value),
        Failed = Convert.ToInt32(failVal.Value),
      };
    }

    public virtual Task NotifySplitCompletion(SplitMessage<TEntity> entity)
    {
      return Task.CompletedTask;
    }
  }
}
