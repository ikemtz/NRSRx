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
    public static IEnumerable<SplitMessage<T>> Create(Func<T> entityFactory, int messageCount = 2)
    {
      return Enumerable.Range(0, messageCount).Select(i =>
        new SplitMessage<T>(entityFactory())
        {
          TaskIndex = i,
          TaskCount = messageCount,
          QueuedBy = "NRSRx Test User",
        });
    }
  }
}
