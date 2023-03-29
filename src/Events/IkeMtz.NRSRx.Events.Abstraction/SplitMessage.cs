using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Events.Abstraction
{
  public class SplitMessage<TEntity> : IIdentifiable<Guid>
        where TEntity : IIdentifiable<Guid>
  {
    public SplitMessage() { }
    public SplitMessage(TEntity entity)
    {
      Entity = entity;
    }
    public Guid Id { get; set; }
    public TEntity Entity { get; set; }
    public string TaskName { get; set; }
    public int TaskCount { get; set; }
    public string QueuedBy { get; set; }

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
