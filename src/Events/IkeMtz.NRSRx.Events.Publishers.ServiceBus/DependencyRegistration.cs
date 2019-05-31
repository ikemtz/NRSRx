using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Events.Publishers.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceBusQueueDependencyRegistration
    {
        public static void AddServiceBusQueuePublishers<Entity>(this IServiceCollection services)
        where Entity : IIdentifiable
        {
            services.AddSingleton<IPublisher<Entity, CreateEvent, Message>>(t =>
                 new ServiceBusQueuePublisher<Entity, CreateEvent>(t.GetService<IConfiguration>()));
            services.AddSingleton<IPublisher<Entity, CreatedEvent, Message>>(t =>
                 new ServiceBusQueuePublisher<Entity, CreatedEvent>(t.GetService<IConfiguration>()));
            services.AddSingleton<IPublisher<Entity, UpdatedEvent, Message>>(t =>
                new ServiceBusQueuePublisher<Entity, UpdatedEvent>(t.GetService<IConfiguration>()));
            services.AddSingleton<IPublisher<Entity, DeletedEvent, Message>>(t =>
                new ServiceBusQueuePublisher<Entity, DeletedEvent>(t.GetService<IConfiguration>()));
        }

        public static void AddServiceBusQueuePublishers<Entity, IdentityType>(this IServiceCollection services)
      where Entity : IIdentifiable<IdentityType>
        {
            services.AddSingleton<IPublisher<Entity, CreateEvent, Message, IdentityType>>(t =>
                new ServiceBusQueuePublisher<Entity, CreateEvent, IdentityType>(t.GetService<IConfiguration>()));
            services.AddSingleton<IPublisher<Entity, CreatedEvent, Message, IdentityType>>(t =>
                new ServiceBusQueuePublisher<Entity, CreatedEvent, IdentityType>(t.GetService<IConfiguration>()));
            services.AddSingleton<IPublisher<Entity, UpdatedEvent, Message, IdentityType>>(t =>
                new ServiceBusQueuePublisher<Entity, UpdatedEvent, IdentityType>(t.GetService<IConfiguration>()));
            services.AddSingleton<IPublisher<Entity, DeletedEvent, Message, IdentityType>>(t =>
                new ServiceBusQueuePublisher<Entity, DeletedEvent, IdentityType>(t.GetService<IConfiguration>()));
        }
    }
}