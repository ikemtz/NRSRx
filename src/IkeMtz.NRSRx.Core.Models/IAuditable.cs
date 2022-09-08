using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.NRSRx.Core.Models
{
  public interface IAuditable<TDATETIME> where TDATETIME : struct
  {
    [Required]
    [MaxLength(250)]
    string CreatedBy { get; set; }
    [MaxLength(250)]
    string? UpdatedBy { get; set; }
    TDATETIME CreatedOnUtc { get; set; }
    TDATETIME? UpdatedOnUtc { get; set; }
  }
  public interface IAuditable : IAuditable<DateTimeOffset>
  {
  }
}
