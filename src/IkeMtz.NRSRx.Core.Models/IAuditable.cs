using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Defines a contract for an objects that can be audited and has a primary key of date-time type.
  /// When used in an IAuditable DbContext, these properties will be automatically be set when record is created or updated.
  /// </summary>
  /// <typeparam name="TDATETIME">The type of the date-time.</typeparam>
  public interface IAuditable<TDATETIME> where TDATETIME : struct
  {
    /// <summary>
    /// Gets or sets the identifier of the user who created the object.
    /// </summary>
    [Required]
    [MaxLength(250)]
    string CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last updated the object.
    /// </summary>
    [MaxLength(250)]
    string? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the number of times the object has been updated.
    /// </summary>
    int? UpdateCount { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the object was created in UTC.
    /// </summary>
    TDATETIME CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the object was last updated in UTC.
    /// </summary>
    TDATETIME? UpdatedOnUtc { get; set; }
  }

  /// <summary>
  /// Defines a contract for objects that can be audited with a DateTimeOffset type.
  /// </summary>
  public interface IAuditable : IAuditable<DateTimeOffset>
  {
  }
}
