using System;

namespace IkeMtz.NRSRx.Core.Models
{
    public interface IIdentifiable : IIdentifiable<Guid>
    {
    }

    public interface IIdentifiable<out TIdentityType>
    {
        TIdentityType Id { get; }
    }
}

