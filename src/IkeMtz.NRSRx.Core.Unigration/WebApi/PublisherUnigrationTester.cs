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
    public Mock<IPublisher<TEntity, CreateEvent>> GuidCreatePublisher { get; }
    public Mock<IPublisher<TEntity, CreatedEvent>> GuidCreatedPublisher { get; }
    public Mock<IPublisher<TEntity, UpdatedEvent>> GuidUpdatedPublisher { get; }
    public Mock<IPublisher<TEntity, DeletedEvent>> GuidDeletedPublisher { get; }

    public PublisherUnigrationTester() : base()
    {
      GuidCreatePublisher = new Mock<IPublisher<TEntity, CreateEvent>>();
      GuidCreatedPublisher = new Mock<IPublisher<TEntity, CreatedEvent>>();
      GuidUpdatedPublisher = new Mock<IPublisher<TEntity, UpdatedEvent>>();
      GuidDeletedPublisher = new Mock<IPublisher<TEntity, DeletedEvent>>();

      _ = GuidCreatePublisher
          .Setup(t => t.PublishAsync(Capture.In(CreateList)))
          .Returns(Task.FromResult(true));
      _ = GuidCreatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(CreatedList)))
          .Returns(Task.FromResult(true));
      _ = GuidUpdatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(UpdatedList)))
          .Returns(Task.FromResult(true));
      _ = GuidDeletedPublisher
          .Setup(t => t.PublishAsync(Capture.In(DeletedList)))
          .Returns(Task.FromResult(true));
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
    public Mock<IPublisher<TEntity, CreateEvent, TIdentityType>> CreatePublisher { get; }
    public Mock<IPublisher<TEntity, CreatedEvent, TIdentityType>> CreatedPublisher { get; }
    public Mock<IPublisher<TEntity, UpdatedEvent, TIdentityType>> UpdatedPublisher { get; }
    public Mock<IPublisher<TEntity, DeletedEvent, TIdentityType>> DeletedPublisher { get; }

    public PublisherUnigrationTester()
    {
      CreatePublisher = new Mock<IPublisher<TEntity, CreateEvent, TIdentityType>>();
      CreatedPublisher = new Mock<IPublisher<TEntity, CreatedEvent, TIdentityType>>();
      UpdatedPublisher = new Mock<IPublisher<TEntity, UpdatedEvent, TIdentityType>>();
      DeletedPublisher = new Mock<IPublisher<TEntity, DeletedEvent, TIdentityType>>();

      _ = CreatePublisher
          .Setup(t => t.PublishAsync(Capture.In(CreateList)))
          .Returns(Task.FromResult(true));
      _ = CreatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(CreatedList)))
          .Returns(Task.FromResult(true));
      _ = UpdatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(UpdatedList)))
          .Returns(Task.FromResult(true));
      _ = DeletedPublisher
          .Setup(t => t.PublishAsync(Capture.In(DeletedList)))
          .Returns(Task.FromResult(true));
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
