using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Defines a contract for objects that can be marked as deleted.
  /// If used within an IAuditable DbContext, these properties will be automatically be set when record is deleted.
  /// </summary>
  public interface IDeletable
  {
    /// <summary>
    /// Gets or sets the date and time when the object was deleted in UTC.
    /// </summary>
    DateTimeOffset? DeletedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who deleted the object.
    /// </summary>
    [MaxLength(256)]
    string? DeletedBy { get; set; }
  }
}
