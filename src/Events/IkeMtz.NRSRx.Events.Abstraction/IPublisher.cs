using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Events
{
  public interface IPublisher<TEntity, TEvent, out TMessageType> :
        IPublisher<TEntity, TEvent, TMessageType, Guid>
        where TEntity : IIdentifiable<Guid>
        where TEvent : EventType, new()
  {
  }

  public interface IPublisher<TEntity, TEvent, out TMessageType, TIdentityType>
  where TEntity : IIdentifiable<TIdentityType>
  where TEvent : EventType, new()
  {
    Task PublishAsync(TEntity payload, Action<TMessageType> messageCustomizationLogic = null);
  }
}
