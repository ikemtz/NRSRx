using System;
using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
  [ExcludeFromCodeCoverage]
  public static class ServiceBusQueueDependencyRegistration
  {
    public static void AddServiceBusQueuePublishers<TEntity>(this IServiceCollection services)
    where TEntity : IIdentifiable
    {
      _ = services.AddSingleton<IPublisher<TEntity, CreatedEvent>>(t =>
           new ServiceBusQueuePublisher<TEntity, CreatedEvent>(t.GetService<IConfiguration>()))
        .AddSingleton<IPublisher<TEntity, UpdatedEvent>>(t =>
          new ServiceBusQueuePublisher<TEntity, UpdatedEvent>(t.GetService<IConfiguration>()))
        .AddSingleton<IPublisher<TEntity, DeletedEvent>>(t =>
          new ServiceBusQueuePublisher<TEntity, DeletedEvent>(t.GetService<IConfiguration>()));
    }

    public static void AddServiceBusQueuePublishers<TEntity, TIdentityType>(this IServiceCollection services)
      where TIdentityType : IComparable
      where TEntity : IIdentifiable<TIdentityType>
    {
      _ = services.AddSingleton<IPublisher<TEntity, CreatedEvent, TIdentityType>>(t =>
          new ServiceBusQueuePublisher<TEntity, CreatedEvent, TIdentityType>(t.GetService<IConfiguration>()))
        .AddSingleton<IPublisher<TEntity, UpdatedEvent, TIdentityType>>(t =>
          new ServiceBusQueuePublisher<TEntity, UpdatedEvent, TIdentityType>(t.GetService<IConfiguration>()))
        .AddSingleton<IPublisher<TEntity, DeletedEvent, TIdentityType>>(t =>
          new ServiceBusQueuePublisher<TEntity, DeletedEvent, TIdentityType>(t.GetService<IConfiguration>()));
    }
  }
}
