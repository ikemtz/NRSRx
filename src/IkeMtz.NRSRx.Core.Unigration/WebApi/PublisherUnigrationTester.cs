using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi
{
  /// <summary>
  /// Provides a tester for publisher unigration with a specific entity and message type, using a GUID as the identity type.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TMessageType">The type of the message.</typeparam>
  public class PublisherUnigrationTester<TEntity, TMessageType> :
      PublisherUnigrationTester<TEntity, TMessageType, Guid>
         where TEntity : IIdentifiable
  {
    /// <summary>
    /// Gets the mock publisher for create events.
    /// </summary>
    public Mock<IPublisher<TEntity, CreateEvent>> GuidCreatePublisher { get; }

    /// <summary>
    /// Gets the mock publisher for created events.
    /// </summary>
    public Mock<IPublisher<TEntity, CreatedEvent>> GuidCreatedPublisher { get; }

    /// <summary>
    /// Gets the mock publisher for updated events.
    /// </summary>
    public Mock<IPublisher<TEntity, UpdatedEvent>> GuidUpdatedPublisher { get; }

    /// <summary>
    /// Gets the mock publisher for deleted events.
    /// </summary>
    public Mock<IPublisher<TEntity, DeletedEvent>> GuidDeletedPublisher { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherUnigrationTester{TEntity, TMessageType}"/> class.
    /// </summary>
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

    /// <summary>
    /// Registers the dependencies for the publishers.
    /// </summary>
    /// <param name="services">The service collection.</param>
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

  /// <summary>
  /// Provides a tester for publisher unigration with a specific entity, message type, and identity type.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TMessageType">The type of the message.</typeparam>
  /// <typeparam name="TIdentityType">The type of the identity.</typeparam>
  public class PublisherUnigrationTester<TEntity, TMessageType, TIdentityType> : PublisherUnigrationTester<TEntity>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
  {
    /// <summary>
    /// Gets the mock publisher for create events.
    /// </summary>
    public Mock<IPublisher<TEntity, CreateEvent, TIdentityType>> CreatePublisher { get; }

    /// <summary>
    /// Gets the mock publisher for created events.
    /// </summary>
    public Mock<IPublisher<TEntity, CreatedEvent, TIdentityType>> CreatedPublisher { get; }

    /// <summary>
    /// Gets the mock publisher for updated events.
    /// </summary>
    public Mock<IPublisher<TEntity, UpdatedEvent, TIdentityType>> UpdatedPublisher { get; }

    /// <summary>
    /// Gets the mock publisher for deleted events.
    /// </summary>
    public Mock<IPublisher<TEntity, DeletedEvent, TIdentityType>> DeletedPublisher { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherUnigrationTester{TEntity, TMessageType, TIdentityType}"/> class.
    /// </summary>
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

    /// <summary>
    /// Registers the dependencies for the publishers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public virtual void RegisterDependencies(IServiceCollection services)
    {
      _ = services.AddSingleton(CreatePublisher.Object)
       .AddSingleton(CreatedPublisher.Object)
       .AddSingleton(UpdatedPublisher.Object)
       .AddSingleton(DeletedPublisher.Object);
    }
  }

  /// <summary>
  /// Provides a base class for publisher unigration testers.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  public abstract class PublisherUnigrationTester<TEntity>
  {
    /// <summary>
    /// Gets the list of entities for create events.
    /// </summary>
    public List<TEntity> CreateList { get; } = new();

    /// <summary>
    /// Gets the list of entities for created events.
    /// </summary>
    public List<TEntity> CreatedList { get; } = new();

    /// <summary>
    /// Gets the list of entities for updated events.
    /// </summary>
    public List<TEntity> UpdatedList { get; } = new();

    /// <summary>
    /// Gets the list of entities for deleted events.
    /// </summary>
    public List<TEntity> DeletedList { get; } = new();
  }
}
