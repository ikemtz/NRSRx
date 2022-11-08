using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Jobs;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Subscribers.Redis;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

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

    public MessageFunction(ILogger<TMessageFunction> logger, RedisStreamSubscriber<TEntity, TEvent, TIdentityType> subscriber)
    {
      Logger = logger;
      Subscriber = subscriber;
    }

    public async Task<bool> RunAsync()
    {
      Logger.LogInformation("Pulling {MessageBufferCount} messages from queue.", MessageBufferCount);
      var messages = await Subscriber.GetMessagesAsync(MessageBufferCount);
      var messageCount = messages.Count();
      Logger.LogInformation("Received {messageCount} messages from queue.", messageCount);
      foreach (var (redisId, entity) in messages)
      {
        var id = entity.Id;
        try
        {
          Logger.LogInformation("Handling message with entity id: {id} ", id);
          await HandleMessageAsync(entity);
          Logger.LogInformation("Handled message with entity id: {id} ", id);
          await Subscriber.AcknowledgeMessageAsync(redisId);
        }
        catch (Exception x)
        {
          Logger.LogError(x, "An error while handling entity id: {id} ", id);
          throw;
        }
      }
      return true;
    }

    public abstract Task HandleMessageAsync(TEntity entity);
  }
}
