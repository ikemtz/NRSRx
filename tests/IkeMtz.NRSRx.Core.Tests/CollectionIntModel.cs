using System;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class CollectionIntModel : IIdentifiable<int>
  {
    public CollectionIntModel()
    {
      Value = Guid.NewGuid().ToString();
    }
    public int Id { get; set; }
    public string Value { get; set; }

    public CollectionIntModel Clone()
    {
      return new CollectionIntModel
      {
        Id = Id,
        Value = Value
      };
    }
  }

  public class CollectionIntModelDto : IIdentifiable<int>
  {
    public CollectionIntModelDto()
    {
      Value = Guid.NewGuid().ToString();
    }
    public int Id { get; set; }
    public string Value { get; set; }

    public CollectionIntModel ToCollectionModel()
    {
      return new CollectionIntModel
      {
        Id = Id,
        Value = Value
      };
    }
  }
}
