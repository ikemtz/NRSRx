using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Defines a contract for objects that have a unique identifier of type <see cref="Guid"/>.
  /// </summary>
  public interface IIdentifiable : IIdentifiable<Guid>
  {
  }

  /// <summary>
  /// Defines a contract for objects that have a unique identifier of a specified type.
  /// </summary>
  /// <typeparam name="TIdentityType">The type of the unique identifier.</typeparam>
  public interface IIdentifiable<TIdentityType> where TIdentityType : IComparable
  {
    /// <summary>
    /// Gets or sets the unique identifier for the object.
    /// </summary>
    [Key]
    TIdentityType Id { get; set; }
  }
}

