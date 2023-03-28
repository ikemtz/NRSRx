using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Events.Abstraction;

namespace IkeMtz.NRSRx.Core.Unigration.Events
{
  public static class SplitMessageFactory<TEntity>
    where TEntity : class, IIdentifiable<Guid>
  {
    public static IEnumerable<SplitMessage<TEntity>> Create(Func<TEntity> entityFactory, int messageCount = 1, string taskName = "Unigration Test Task", string userName = "NRSRx Test User")
    {
      return SplitMessage<TEntity>.FromCollection(Enumerable.Range(0, messageCount).Select(i => entityFactory()), taskName, userName);
    }
  }
}
