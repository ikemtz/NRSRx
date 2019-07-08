using IkeMtz.NRSRx.Core.Models;
using System;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class CollectionModel : IIdentifiable
  {
    public CollectionModel()
    {
      Id = Guid.NewGuid();
      Value = Guid.NewGuid().ToString();
    }
    public Guid Id { get;  }
    public string Value { get; set; }
  }
}
