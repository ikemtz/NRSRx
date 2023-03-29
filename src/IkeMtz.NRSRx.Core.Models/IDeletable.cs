using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.NRSRx.Core.Models
{
  public interface IDeletable
  {
    DateTimeOffset? DeletedOnUtc { get; set; }
    [MaxLength(256)]
    string? DeletedBy { get; set; }
  }
}
