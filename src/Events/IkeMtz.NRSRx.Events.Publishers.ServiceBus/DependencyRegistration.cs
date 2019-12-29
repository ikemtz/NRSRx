using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
  public static class ServiceBusQueueDependencyRegistration
  {
    public static void AddServiceBusQueuePublishers<TEntity>(this IServiceCollection services)
    where TEntity : IIdentifiable
    {
      services.AddSingleton<IPublisher<TEntity, CreatedEvent, Message>>(t =>
           new ServiceBusQueuePublisher<TEntity, CreatedEvent>(t.GetService<IConfiguration>()));
      services.AddSingleton<IPublisher<TEntity, UpdatedEvent, Message>>(t =>
          new ServiceBusQueuePublisher<TEntity, UpdatedEvent>(t.GetService<IConfiguration>()));
      services.AddSingleton<IPublisher<TEntity, DeletedEvent, Message>>(t =>
          new ServiceBusQueuePublisher<TEntity, DeletedEvent>(t.GetService<IConfiguration>()));
    }

    public static void AddServiceBusQueuePublishers<TEntity, TIdentityType>(this IServiceCollection services)
  where TEntity : IIdentifiable<TIdentityType>
    {
      services.AddSingleton<IPublisher<TEntity, CreatedEvent, Message, TIdentityType>>(t =>
          new ServiceBusQueuePublisher<TEntity, CreatedEvent, TIdentityType>(t.GetService<IConfiguration>()));
      services.AddSingleton<IPublisher<TEntity, UpdatedEvent, Message, TIdentityType>>(t =>
          new ServiceBusQueuePublisher<TEntity, UpdatedEvent, TIdentityType>(t.GetService<IConfiguration>()));
      services.AddSingleton<IPublisher<TEntity, DeletedEvent, Message, TIdentityType>>(t =>
          new ServiceBusQueuePublisher<TEntity, DeletedEvent, TIdentityType>(t.GetService<IConfiguration>()));
    }
  }
}
