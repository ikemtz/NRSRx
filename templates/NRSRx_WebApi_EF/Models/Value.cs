using System;
using IkeMtz.NRSRx.Core.Models;

namespace NRSRx_WebApi_EF.Models
{
  public class Value : IIdentifiable, IAuditable
  {
    public Guid Id { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }
  }
}
