using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi
{
    public class PublisherIntegrationTester<Entity, MessageType>
        where Entity: IIdentifiable
    {
        public readonly Mock<IPublisher<Entity, CreateEvent, MessageType>> CreatePublisher;
        public readonly Mock<IPublisher<Entity, CreatedEvent, MessageType>> CreatedPublisher;
        public readonly Mock<IPublisher<Entity, UpdatedEvent, MessageType>> UpdatedPublisher;
        public readonly Mock<IPublisher<Entity, DeletedEvent, MessageType>> DeletedPublisher;

        public static readonly List<Entity> CreateList = new List<Entity>();
        public static readonly List<Entity> CreatedList = new List<Entity>();
        public static readonly List<Entity> UpdatedList = new List<Entity>();
        public static readonly List<Entity> DeletedList = new List<Entity>();

        public PublisherIntegrationTester()
        {
            CreatePublisher = new Mock<IPublisher<Entity, CreateEvent, MessageType>>();
            CreatedPublisher = new Mock<IPublisher<Entity, CreatedEvent, MessageType>>();
            UpdatedPublisher = new Mock<IPublisher<Entity, UpdatedEvent, MessageType>>();
            DeletedPublisher = new Mock<IPublisher<Entity, DeletedEvent, MessageType>>();

            CreatePublisher
                .Setup(t => t.PublishAsync(Capture.In(CreateList), null))
                .Returns(Task.CompletedTask);
            CreatedPublisher
                .Setup(t => t.PublishAsync(Capture.In(CreatedList), null))
                .Returns(Task.CompletedTask);
            UpdatedPublisher
                .Setup(t => t.PublishAsync(Capture.In(UpdatedList), null))
                .Returns(Task.CompletedTask);
            DeletedPublisher
                .Setup(t => t.PublishAsync(Capture.In(DeletedList), null))
                .Returns(Task.CompletedTask);
        }

        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddSingleton(CreatePublisher.Object);
            services.AddSingleton(CreatedPublisher.Object);
            services.AddSingleton(UpdatedPublisher.Object);
            services.AddSingleton(DeletedPublisher.Object);
        }
    }
}

