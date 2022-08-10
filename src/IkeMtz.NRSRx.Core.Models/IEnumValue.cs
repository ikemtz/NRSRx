using System;

namespace IkeMtz.NRSRx.Core.Models
{
  public interface IEnumValue : IEnumValue<int>
  {
  }
  public interface IEnumValue<TIdentityType> : IIdentifiable<TIdentityType>
    where TIdentityType : IComparable
  {
    public string Name { get; set; }
  }
}
