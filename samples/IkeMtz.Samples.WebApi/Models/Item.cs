using System;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.Samples.WebApi.Models
{
  public class Item : IIdentifiable, IAuditable
  {
    public Guid Id { get; set; }
    public string Value { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }
  }
}
