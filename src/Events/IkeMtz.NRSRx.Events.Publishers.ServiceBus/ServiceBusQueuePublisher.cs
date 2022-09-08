using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.Events.Publishers.ServiceBus
{
  public class ServiceBusQueuePublisher<TEntity, TEvent> :
      ServiceBusQueuePublisher<TEntity, TEvent, Guid>,
      IPublisher<TEntity, TEvent, ServiceBusMessage>
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

  [ExcludeFromCodeCoverage]
  public class ServiceBusQueuePublisher<TEntity, TEvent, TIdentityType> :
      IPublisher<TEntity, TEvent, ServiceBusMessage, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    private readonly ServiceBusSender queueClient;
    public ServiceBusQueuePublisher(IConfiguration configuration)
    {
      var connectionStringName = $"{GetQueueName().Replace("-", "")}QueConnStr";
      var connectionString = configuration.GetValue<string>(connectionStringName);
      if (string.IsNullOrWhiteSpace(connectionString))
      {
        throw new ConnectionStringMissingException(connectionStringName);
      }
      var busClient = new ServiceBusClient(connectionString);
      queueClient = busClient.CreateSender(GetQueueName());
    }

    public ServiceBusQueuePublisher(string queueConnectionString)
    {
      var busClient = new ServiceBusClient(queueConnectionString);
      queueClient = busClient.CreateSender(GetQueueName());
    }

    public Task PublishAsync(TEntity payload, Action<ServiceBusMessage>? messageCustomizationLogic = null)
    {
      var msg = new ServiceBusMessage(MessageCoder.JsonEncode(payload));
      messageCustomizationLogic?.Invoke(msg);
      return queueClient.SendMessageAsync(msg);
    }

    private static string GetQueueName()
    {
      var eventType = new TEvent();
      return $"{typeof(TEntity).Name}{eventType.EventSuffix}";
    }
  }
}
