using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Events
{
  public interface ISimplePublisher<TEntity, TEvent, TPublishAck> :
        ISimplePublisher<TEntity, TEvent, TPublishAck, Guid>
        where TEntity : IIdentifiable<Guid>
        where TEvent : EventType, new()
  {
  }

  public interface ISimplePublisher<TEntity, TEvent, TPublishAck, TIdentityType>
    where TIdentityType : IComparable
    where TEntity : IIdentifiable<TIdentityType>
    where TEvent : EventType, new()
  {
    Task<TPublishAck> PublishAsync(TEntity payload);
  }
}
