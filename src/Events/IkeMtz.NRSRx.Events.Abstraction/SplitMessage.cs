using System;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Events.Abstraction
{
  public class SplitMessage<TEntity> : IIdentifiable<Guid>
        where TEntity : IIdentifiable<Guid>
  {
    public SplitMessage() { }
    public SplitMessage(TEntity entity)
    {
      Id = entity.Id;
      Entity = entity;
    }
    public Guid Id { get; set; }
    public TEntity Entity { get; set; }
    public int TaskIndex { get; set; }
    public int TaskCount { get; set; }

  }
}
