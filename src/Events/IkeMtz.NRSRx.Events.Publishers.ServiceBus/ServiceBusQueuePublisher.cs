using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.Events.Publishers.ServiceBus
{
  public class ServiceBusQueuePublisher<TEntity, TEvent> :
      ServiceBusQueuePublisher<TEntity, TEvent, Guid>,
      IPublisher<TEntity, TEvent, Message>
   where TEntity : IIdentifiable<Guid>
   where TEvent : EventType, new()
  {
    public ServiceBusQueuePublisher(IConfiguration configuration) : base(configuration)
    {
    }

    public ServiceBusQueuePublisher(string connectionString) : base(connectionString)
    {
    }
  }

  public class ServiceBusQueuePublisher<TEntity, TEvent, TIdentityType> :
      IPublisher<TEntity, TEvent, Message, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    private readonly QueueClient queueClient;
    public ServiceBusQueuePublisher(IConfiguration configuration)
    {
      var connectionStringName = $"{GetQueueName().Replace("-", "")}QueConnStr";
      var connectionString = configuration.GetValue<string>(connectionStringName);
      if (string.IsNullOrWhiteSpace(connectionString))
      {
        throw new ConnectionStringMissingException(connectionStringName);
      }
      queueClient = new QueueClient(connectionString, GetQueueName(), ReceiveMode.PeekLock);

    }

    public ServiceBusQueuePublisher(string queueConnectionString)
    {
      queueClient = new QueueClient(queueConnectionString, GetQueueName(), ReceiveMode.PeekLock);
    }

    public Task PublishAsync(TEntity payload, Action<Message> messageCustomizationLogic = null)
    {
      var msg = new Message(MessageCoder.JsonEncode(payload));
      messageCustomizationLogic?.Invoke(msg);
      return queueClient.SendAsync(msg);
    }

    private static string GetQueueName()
    {
      var eventType = new TEvent();
      return $"{typeof(TEntity).Name}{eventType.EventSuffix}";
    }
  }
}
