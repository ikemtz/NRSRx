using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;

namespace IkeMtz.NRSRx.Core.Unigration.Events
{
  public static class SplitMessageFactory<T>
    where T : class, IIdentifiable<Guid>
  {
    public static IEnumerable<SplitMessage<T>> Create(Func<T> entityFactory, int messageCount)
    {
      return Enumerable.Range(0, messageCount).Select(i => Create(entityFactory, i, messageCount));
    }

    public static SplitMessage<T> Create(Func<T> entityFactory, int index = 1, int messageCount = 1)
    {
      return new SplitMessage<T>(entityFactory())
      {
        TaskIndex = index,
        TaskCount = messageCount,
        QueuedBy = "NRSRx Test User",
      };
    }
  }
}
