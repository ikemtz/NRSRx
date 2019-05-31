using IkeMtz.NRSRx.Core.Models;
using System;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Events
{
    public interface IPublisher<Entity, Event, out MessageType> :
        IPublisher<Entity, Event, MessageType, Guid>
        where Entity : IIdentifiable<Guid>
        where Event : EventType, new()
    {
    }

    public interface IPublisher<Entity, Event, out MessageType, IdentityType>
    where Entity : IIdentifiable<IdentityType>
    where Event : EventType, new()
    {
        Task PublishAsync(Entity payload, Action<MessageType> messageCustomizationLogic = null);
    }
}