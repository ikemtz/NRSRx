using System;

namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Represents an enum value with an integer identity.
  /// </summary>
  public interface IEnumValue : IEnumValue<int>
  {
  }

  /// <summary>
  /// Represents an enum value with a specified identity type.
  /// </summary>
  /// <typeparam name="TIdentityType">The type of the identity.</typeparam>
  public interface IEnumValue<TIdentityType> : IIdentifiable<TIdentityType>
    where TIdentityType : IComparable
  {
    /// <summary>
    /// Gets or sets the name of the enum value.
    /// </summary>
    public string Name { get; set; }
  }
}
