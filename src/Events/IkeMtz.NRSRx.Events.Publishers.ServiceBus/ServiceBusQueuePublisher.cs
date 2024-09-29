using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.Events.Publishers.ServiceBus
{
  /// <summary>
  /// Service Bus queue publisher for entities with a <see cref="Guid"/> identifier.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  public class ServiceBusQueuePublisher<TEntity, TEvent> :
        ServiceBusQueuePublisher<TEntity, TEvent, Guid>,
        IPublisher<TEntity, TEvent>
     where TEntity : IIdentifiable<Guid>
     where TEvent : EventType, new()
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusQueuePublisher{TEntity, TEvent}"/> class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
    public ServiceBusQueuePublisher(IConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusQueuePublisher{TEntity, TEvent}"/> class with the specified connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    public ServiceBusQueuePublisher(string connectionString) : base(connectionString)
    {
    }
  }

  /// <summary>
  /// Service Bus queue publisher for entities.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  /// <typeparam name="TIdentityType">The type of the entity identifier.</typeparam>
  [ExcludeFromCodeCoverage]
  public class ServiceBusQueuePublisher<TEntity, TEvent, TIdentityType> :
      IPublisher<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    private readonly ServiceBusSender queueClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusQueuePublisher{TEntity, TEvent, TIdentityType}"/> class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration to use.</param>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusQueuePublisher{TEntity, TEvent, TIdentityType}"/> class with the specified connection string.
    /// </summary>
    /// <param name="queueConnectionString">The connection string to use.</param>
    public ServiceBusQueuePublisher(string queueConnectionString)
    {
      var busClient = new ServiceBusClient(queueConnectionString);
      queueClient = busClient.CreateSender(GetQueueName());
    }

    /// <summary>
    /// Publishes an event asynchronously.
    /// </summary>
    /// <param name="payload">The entity payload.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    public Task PublishAsync(TEntity payload)
    {
      var msg = new ServiceBusMessage(MessageCoder.JsonEncode(payload));
      return queueClient.SendMessageAsync(msg);
    }

    /// <summary>
    /// Gets the name of the queue.
    /// </summary>
    /// <returns>The name of the queue.</returns>
    private static string GetQueueName()
    {
      var eventType = new TEvent();
      return $"{typeof(TEntity).Name}{eventType.EventSuffix}";
    }
  }
}
