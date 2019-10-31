using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Events.Publishers.ServiceBus
{
  public class ServiceBusQueuePublisher<Entity, Event> :
      ServiceBusQueuePublisher<Entity, Event, Guid>,
      IPublisher<Entity, Event, Message>
   where Entity : IIdentifiable<Guid>
   where Event : EventType, new()
  {
    public ServiceBusQueuePublisher(IConfiguration configuration) : base(configuration)
    {
    }

    public ServiceBusQueuePublisher(string connectionString) : base(connectionString)
    {
    }
  }

  public class ServiceBusQueuePublisher<Entity, Event, IdentityType> :
      IPublisher<Entity, Event, Message, IdentityType>
  where Entity : IIdentifiable<IdentityType>
  where Event : EventType, new()
  {
    private readonly QueueClient queueClient;
    public ServiceBusQueuePublisher(IConfiguration configuration)
    {
      queueClient = new QueueClient(configuration.GetValue<string>($"{getQueueName()}QueConnStr"), getQueueName(), ReceiveMode.PeekLock);
    }

    public ServiceBusQueuePublisher(string queueConnectionString)
    {
      queueClient = new QueueClient(queueConnectionString, getQueueName(), ReceiveMode.PeekLock);
    }

    public Task PublishAsync(Entity payload, Action<Message> messageCustomizationLogic = null)
    {
      var json = JsonConvert.SerializeObject(payload, Constants.JsonSerializerSettings);
      var buffer = Encoding.UTF8.GetBytes(json);
      var msg = new Message(buffer);
      messageCustomizationLogic?.Invoke(msg);
      return queueClient.SendAsync(msg);
    }

    private string getQueueName()
    {
      var eventType = new Event();
      return $"{typeof(Entity).Name}{eventType.EventSuffix}";
    }
  }
}
