using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.NRSRx.Core.Models
{
  public interface IIdentifiable : IIdentifiable<Guid>
  {
  }

  public interface IIdentifiable<TIdentityType> where TIdentityType : IComparable
  {
    [Key]
    TIdentityType Id { get; set; }
  }
}

