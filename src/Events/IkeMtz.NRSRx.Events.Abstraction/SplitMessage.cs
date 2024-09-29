using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Events.Abstraction
{
  /// <summary>
  /// Represents a message that is from a collection of messages, generated off a single event.
  /// This is used to support the Fan Out pattern.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  public class SplitMessage<TEntity> : IIdentifiable<Guid>
          where TEntity : IIdentifiable<Guid>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SplitMessage{TEntity}"/> class.
    /// </summary>
    public SplitMessage() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SplitMessage{TEntity}"/> class with the specified entity.
    /// </summary>
    /// <param name="entity">The entity to include in the message.</param>
    public SplitMessage(TEntity entity)
    {
      Entity = entity;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the message.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the entity included in the message.
    /// </summary>
    public TEntity Entity { get; set; }

    /// <summary>
    /// Gets or sets the name of the task associated with the message.
    /// </summary>
    public string TaskName { get; set; }

    /// <summary>
    /// Gets or sets the total number of tasks.
    /// </summary>
    public int TaskCount { get; set; }

    /// <summary>
    /// Gets or sets the user who queued the message.
    /// </summary>
    public string QueuedBy { get; set; }

    /// <summary>
    /// Creates a collection of <see cref="SplitMessage{TEntity}"/> instances from a collection of entities.
    /// </summary>
    /// <param name="collection">The collection of entities.</param>
    /// <param name="taskName">The name of the task.</param>
    /// <param name="userName">The name of the user who queued the messages.</param>
    /// <returns>A collection of <see cref="SplitMessage{TEntity}"/> instances.</returns>
    public static IEnumerable<SplitMessage<TEntity>> FromCollection(IEnumerable<TEntity> collection, string taskName, string userName)
    {
      var taskId = Guid.NewGuid();
      var count = collection.Count();
      return collection.Select(entity => new SplitMessage<TEntity>(entity)
      {
        Id = taskId,
        TaskName = taskName,
        TaskCount = count,
        QueuedBy = userName,
      });
    }
  }
}
