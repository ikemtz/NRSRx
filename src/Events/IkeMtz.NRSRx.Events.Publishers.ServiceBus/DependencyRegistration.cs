using System;
using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
  /// <summary>
  /// Provides extension methods for registering Service Bus queue publishers.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public static class ServiceBusQueueDependencyRegistration
  {
    /// <summary>
    /// Adds Service Bus queue publishers for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="services">The service collection to add the publishers to.</param>
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

    /// <summary>
    /// Adds Service Bus queue publishers for the specified entity type and identifier type.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TIdentityType">The type of the entity identifier.</typeparam>
    /// <param name="services">The service collection to add the publishers to.</param>
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
