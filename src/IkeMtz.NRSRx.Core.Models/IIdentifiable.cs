using System;

namespace IkeMtz.NRSRx.Core.Models
{
  public interface IIdentifiable : IIdentifiable<Guid>
  {
  }

  public interface IIdentifiable<TIdentityType> where TIdentityType : IComparable
  {
    TIdentityType Id { get; set; }
  }
}

