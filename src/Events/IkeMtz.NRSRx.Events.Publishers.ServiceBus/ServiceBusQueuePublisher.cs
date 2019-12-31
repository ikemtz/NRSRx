using System;
using System.Text;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

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
      var json = JsonConvert.SerializeObject(payload, Constants.JsonSerializerSettings);
      var buffer = Encoding.UTF8.GetBytes(json);
      var msg = new Message(buffer);
      messageCustomizationLogic?.Invoke(msg);
      return queueClient.SendAsync(msg);
    }

    private string GetQueueName()
    {
      var eventType = new TEvent();
      return $"{typeof(TEntity).Name}{eventType.EventSuffix}";
    }
  }
}
