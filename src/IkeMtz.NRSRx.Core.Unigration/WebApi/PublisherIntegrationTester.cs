using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi
{
  public class PublisherIntegrationTester<TEntity, TMessageType> : PublisherIntegrationTester<TEntity>
       where TEntity : IIdentifiable
  {
    public Mock<IPublisher<TEntity, CreateEvent, TMessageType>> CreatePublisher { get; }
    public Mock<IPublisher<TEntity, CreatedEvent, TMessageType>> CreatedPublisher { get; }
    public Mock<IPublisher<TEntity, UpdatedEvent, TMessageType>> UpdatedPublisher { get; }
    public Mock<IPublisher<TEntity, DeletedEvent, TMessageType>> DeletedPublisher { get; }

    public PublisherIntegrationTester()
    {
      CreatePublisher = new Mock<IPublisher<TEntity, CreateEvent, TMessageType>>();
      CreatedPublisher = new Mock<IPublisher<TEntity, CreatedEvent, TMessageType>>();
      UpdatedPublisher = new Mock<IPublisher<TEntity, UpdatedEvent, TMessageType>>();
      DeletedPublisher = new Mock<IPublisher<TEntity, DeletedEvent, TMessageType>>();

      _ = CreatePublisher
          .Setup(t => t.PublishAsync(Capture.In(CreateList), null))
          .Returns(Task.CompletedTask);
      _ = CreatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(CreatedList), null))
          .Returns(Task.CompletedTask);
      _ = UpdatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(UpdatedList), null))
          .Returns(Task.CompletedTask);
      _ = DeletedPublisher
          .Setup(t => t.PublishAsync(Capture.In(DeletedList), null))
          .Returns(Task.CompletedTask);
    }

    public void RegisterDependencies(IServiceCollection services)
    {
      _ = services.AddSingleton(CreatePublisher.Object)
      .AddSingleton(CreatedPublisher.Object)
      .AddSingleton(UpdatedPublisher.Object)
      .AddSingleton(DeletedPublisher.Object);
    }
  }

  public class PublisherIntegrationTester<TEntity, TMessageType, TIdentityType> : PublisherIntegrationTester<TEntity>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
  {
    public Mock<IPublisher<TEntity, CreateEvent, TMessageType, TIdentityType>> CreatePublisher { get; }
    public Mock<IPublisher<TEntity, CreatedEvent, TMessageType, TIdentityType>> CreatedPublisher { get; }
    public Mock<IPublisher<TEntity, UpdatedEvent, TMessageType, TIdentityType>> UpdatedPublisher { get; }
    public Mock<IPublisher<TEntity, DeletedEvent, TMessageType, TIdentityType>> DeletedPublisher { get; }

    public PublisherIntegrationTester()
    {
      CreatePublisher = new Mock<IPublisher<TEntity, CreateEvent, TMessageType, TIdentityType>>();
      CreatedPublisher = new Mock<IPublisher<TEntity, CreatedEvent, TMessageType, TIdentityType>>();
      UpdatedPublisher = new Mock<IPublisher<TEntity, UpdatedEvent, TMessageType, TIdentityType>>();
      DeletedPublisher = new Mock<IPublisher<TEntity, DeletedEvent, TMessageType, TIdentityType>>();

      _ = CreatePublisher
          .Setup(t => t.PublishAsync(Capture.In(CreateList), null))
          .Returns(Task.CompletedTask);
      _ = CreatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(CreatedList), null))
          .Returns(Task.CompletedTask);
      _ = UpdatedPublisher
          .Setup(t => t.PublishAsync(Capture.In(UpdatedList), null))
          .Returns(Task.CompletedTask);
      _ = DeletedPublisher
          .Setup(t => t.PublishAsync(Capture.In(DeletedList), null))
          .Returns(Task.CompletedTask);
    }

    public void RegisterDependencies(IServiceCollection services)
    {
      _ = services.AddSingleton(CreatePublisher.Object)
       .AddSingleton(CreatedPublisher.Object)
       .AddSingleton(UpdatedPublisher.Object)
       .AddSingleton(DeletedPublisher.Object);
    }
  }

  public abstract class PublisherIntegrationTester<TEntity>
  {
    public List<TEntity> CreateList { get; } = new List<TEntity>();
    public List<TEntity> CreatedList { get; } = new List<TEntity>();
    public List<TEntity> UpdatedList { get; } = new List<TEntity>();
    public List<TEntity> DeletedList { get; } = new List<TEntity>();
  }
}

