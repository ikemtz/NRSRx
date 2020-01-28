using System;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class CollectionModel : IIdentifiable
  {
    public CollectionModel()
    {
      Id = Guid.NewGuid();
      Value = Guid.NewGuid().ToString();
    }
    public Guid Id { get; set; }
    public string Value { get; set; }
  }
}
