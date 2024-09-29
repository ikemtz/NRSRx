using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;

namespace IkeMtz.NRSRx.Core.Unigration.Events
{
  /// <summary>
  /// Factory for creating split messages for testing purposes.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  public static class SplitMessageFactory<TEntity>
      where TEntity : class, IIdentifiable<Guid>
  {
    /// <summary>
    /// Creates a collection of split messages.
    /// </summary>
    /// <param name="entityFactory">A function to create an entity.</param>
    /// <param name="messageCount">The number of messages to create. Default is 1.</param>
    /// <param name="taskName">The name of the task. Default is "Unigration Test Task".</param>
    /// <param name="userName">The name of the user who queued the messages. Default is "NRSRx Test User".</param>
    /// <returns>A collection of split messages.</returns>
    public static IEnumerable<SplitMessage<TEntity>> Create(Func<TEntity> entityFactory, int messageCount = 1, string taskName = "Unigration Test Task", string userName = "NRSRx Test User")
    {
      return SplitMessage<TEntity>.FromCollection(Enumerable.Range(0, messageCount).Select(i => entityFactory()), taskName, userName);
    }
  }
}
