using System;

namespace IkeMtz.NRSRx.Core.Models
{
  public interface IAuditable<TDATETIME> where TDATETIME : struct
  {
    string CreatedBy { get; set; }
    string UpdatedBy { get; set; }
    TDATETIME CreatedOnUtc { get; set; }
    TDATETIME? UpdatedOnUtc { get; set; }
  }
  public interface IAuditable : IAuditable<DateTimeOffset>
  {
  }
}
