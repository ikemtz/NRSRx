using System;
using IkeMtz.NRSRx.Core.Models;

namespace NRSRx_OData_EF.Models
{
  public class Item : IIdentifiable
  {
    public Guid Id { get; set; }
    public string Value { get; set; }
  }
}
