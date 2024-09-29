using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Events
{
  /// <summary>
  /// Interface for publishing events for entities with a <see cref="Guid"/> identifier.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  public interface IPublisher<TEntity, TEvent> :
          IPublisher<TEntity, TEvent, Guid>
          where TEntity : IIdentifiable<Guid>
          where TEvent : EventType, new()
  {
  }

  /// <summary>
  /// Interface for publishing events for entities.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TEvent">The type of the event.</typeparam>
  /// <typeparam name="TIdentityType">The type of the entity identifier.</typeparam>
  public interface IPublisher<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    /// <summary>
    /// Publishes an event asynchronously.
    /// </summary>
    /// <param name="payload">The entity payload.</param>
    /// <returns>A task that represents the asynchronous publish operation.</returns>
    Task PublishAsync(TEntity payload);
  }
}
