using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi
{
  public class PublisherUnigrationTester<TEntity, TMessageType> :
    PublisherUnigrationTester<TEntity, TMessageType, Guid>
       where TEntity : IIdentifiable
  {
    public Mock<IPublisher<TEntity, CreateEvent, TMessageType>> GuidCreatePublisher { get; }
    public Mock<IPublisher<TEntity, CreatedEvent, TMessageType>> GuidCreatedPublisher { get; }
    public Mock<IPublisher<TEntity, UpdatedEvent, TMessageType>> GuidUpdatedPublisher { get; }
    public Mock<IPublisher<TEntity, DeletedEvent, TMessageType>> GuidDeletedPublisher { get; }

    public PublisherUnigrationTester() : base()
    {
      GuidCreatePublisher = new Mock<IPublisher<TEntity, CreateEvent, TMessageType>>();
      GuidCreatedPublisher = new Mock<IPublisher<TEntity, CreatedEvent, TMessageType>>();
      GuidUpdatedPublisher = new Mock<IPublisher<TEntity, UpdatedEvent, TMessageType>>();
      GuidDeletedPublisher = new Mock<IPublisher<TEntity, DeletedEvent, TMessageType>>();

      _ = GuidCreatePublisher
          .Setup(t => t.PublishAsync(Capture.In(CreateList), null))
          .Returns(Task.CompletedTask);
      _ = GuidCreatePublisher
          .Setup(t => t.PublishAsync(Capture.In(CreateList), It.IsAny<Action<TMessageType>>()))
          .Returns(Task.CompletedTask);
      _ = GuidCreatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(CreatedList), null))
          .Returns(Task.CompletedTask);
      _ = GuidCreatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(CreatedList), It.IsAny<Action<TMessageType>>()))
          .Returns(Task.CompletedTask);
      _ = GuidUpdatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(UpdatedList), null))
          .Returns(Task.CompletedTask);
      _ = GuidUpdatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(UpdatedList), It.IsAny<Action<TMessageType>>()))
          .Returns(Task.CompletedTask);
      _ = GuidDeletedPublisher
          .Setup(t => t.PublishAsync(Capture.In(DeletedList), null))
          .Returns(Task.CompletedTask);
      _ = GuidDeletedPublisher
          .Setup(t => t.PublishAsync(Capture.In(DeletedList), It.IsAny<Action<TMessageType>>()))
          .Returns(Task.CompletedTask);
    }

    public override void RegisterDependencies(IServiceCollection services)
    {
      base.RegisterDependencies(services);
      _ = services
        .AddSingleton(GuidCreatePublisher.Object)
        .AddSingleton(GuidCreatedPublisher.Object)
        .AddSingleton(GuidUpdatedPublisher.Object)
        .AddSingleton(GuidDeletedPublisher.Object);
    }
  }

  public class PublisherUnigrationTester<TEntity, TMessageType, TIdentityType> : PublisherUnigrationTester<TEntity>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
  {
    public Mock<IPublisher<TEntity, CreateEvent, TMessageType, TIdentityType>> CreatePublisher { get; }
    public Mock<IPublisher<TEntity, CreatedEvent, TMessageType, TIdentityType>> CreatedPublisher { get; }
    public Mock<IPublisher<TEntity, UpdatedEvent, TMessageType, TIdentityType>> UpdatedPublisher { get; }
    public Mock<IPublisher<TEntity, DeletedEvent, TMessageType, TIdentityType>> DeletedPublisher { get; }

    public PublisherUnigrationTester()
    {
      CreatePublisher = new Mock<IPublisher<TEntity, CreateEvent, TMessageType, TIdentityType>>();
      CreatedPublisher = new Mock<IPublisher<TEntity, CreatedEvent, TMessageType, TIdentityType>>();
      UpdatedPublisher = new Mock<IPublisher<TEntity, UpdatedEvent, TMessageType, TIdentityType>>();
      DeletedPublisher = new Mock<IPublisher<TEntity, DeletedEvent, TMessageType, TIdentityType>>();

      _ = CreatePublisher
          .Setup(t => t.PublishAsync(Capture.In(CreateList), null))
          .Returns(Task.CompletedTask);
      _ = CreatePublisher
          .Setup(t => t.PublishAsync(Capture.In(CreateList), It.IsAny<Action<TMessageType>>()))
          .Returns(Task.CompletedTask);
      _ = CreatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(CreatedList), null))
          .Returns(Task.CompletedTask);
      _ = CreatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(CreatedList), It.IsAny<Action<TMessageType>>()))
          .Returns(Task.CompletedTask);
      _ = UpdatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(UpdatedList), null))
          .Returns(Task.CompletedTask);
      _ = UpdatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(UpdatedList), It.IsAny<Action<TMessageType>>()))
          .Returns(Task.CompletedTask);
      _ = DeletedPublisher
          .Setup(t => t.PublishAsync(Capture.In(DeletedList), null))
          .Returns(Task.CompletedTask);
      _ = DeletedPublisher
          .Setup(t => t.PublishAsync(Capture.In(DeletedList), It.IsAny<Action<TMessageType>>()))
          .Returns(Task.CompletedTask);
    }

    public virtual void RegisterDependencies(IServiceCollection services)
    {
      _ = services.AddSingleton(CreatePublisher.Object)
       .AddSingleton(CreatedPublisher.Object)
       .AddSingleton(UpdatedPublisher.Object)
       .AddSingleton(DeletedPublisher.Object);
    }
  }

  public abstract class PublisherUnigrationTester<TEntity>
  {
    public List<TEntity> CreateList { get; } = new List<TEntity>();
    public List<TEntity> CreatedList { get; } = new List<TEntity>();
    public List<TEntity> UpdatedList { get; } = new List<TEntity>();
    public List<TEntity> DeletedList { get; } = new List<TEntity>();
  }
}
