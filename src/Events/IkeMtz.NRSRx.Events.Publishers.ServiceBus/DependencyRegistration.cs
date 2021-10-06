using System;
using Azure.Messaging.ServiceBus;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ServiceBusQueueDependencyRegistration
  {
    public static void AddServiceBusQueuePublishers<TEntity>(this IServiceCollection services)
    where TEntity : IIdentifiable
    {
      _ = services.AddSingleton<IPublisher<TEntity, CreatedEvent, ServiceBusMessage>>(t =>
           new ServiceBusQueuePublisher<TEntity, CreatedEvent>(t.GetService<IConfiguration>()))
        .AddSingleton<IPublisher<TEntity, UpdatedEvent, ServiceBusMessage>>(t =>
          new ServiceBusQueuePublisher<TEntity, UpdatedEvent>(t.GetService<IConfiguration>()))
        .AddSingleton<IPublisher<TEntity, DeletedEvent, ServiceBusMessage>>(t =>
          new ServiceBusQueuePublisher<TEntity, DeletedEvent>(t.GetService<IConfiguration>()));
    }

    public static void AddServiceBusQueuePublishers<TEntity, TIdentityType>(this IServiceCollection services)
      where TIdentityType : IComparable
      where TEntity : IIdentifiable<TIdentityType>
    {
      _ = services.AddSingleton<IPublisher<TEntity, CreatedEvent, ServiceBusMessage, TIdentityType>>(t =>
          new ServiceBusQueuePublisher<TEntity, CreatedEvent, TIdentityType>(t.GetService<IConfiguration>()))
        .AddSingleton<IPublisher<TEntity, UpdatedEvent, ServiceBusMessage, TIdentityType>>(t =>
          new ServiceBusQueuePublisher<TEntity, UpdatedEvent, TIdentityType>(t.GetService<IConfiguration>()))
        .AddSingleton<IPublisher<TEntity, DeletedEvent, ServiceBusMessage, TIdentityType>>(t =>
          new ServiceBusQueuePublisher<TEntity, DeletedEvent, TIdentityType>(t.GetService<IConfiguration>()));
    }
  }
}
