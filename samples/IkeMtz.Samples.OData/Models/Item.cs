using System;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.Samples.OData.Models
{
  public class Item : IIdentifiable
  {
    public Guid Id { get; set; }
    public string Value { get; set; }
  }
}
