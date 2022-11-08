using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Events
{
  public interface IPublisher<TEntity, TEvent> :
        IPublisher<TEntity, TEvent, Guid>
        where TEntity : IIdentifiable<Guid>
        where TEvent : EventType, new()
  {
  }

  public interface IPublisher<TEntity, TEvent, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    Task PublishAsync(TEntity payload);
  }
}
