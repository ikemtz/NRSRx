using System;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class CollectionGuidModel : IIdentifiable
  {
    public CollectionGuidModel()
    {
      Id = Guid.NewGuid();
      Value = Guid.NewGuid().ToString();
    }
    public Guid Id { get; set; }
    public string Value { get; set; }

    public CollectionGuidModel Clone()
    {
      return new CollectionGuidModel
      {
        Id = Id,
        Value = Value
      };
    }
  }

  public class CollectionGuidModelDto : IIdentifiable
  {
    public CollectionGuidModelDto()
    {
      Id = Guid.NewGuid();
      Value = Guid.NewGuid().ToString();
    }
    public Guid Id { get; set; }
    public string Value { get; set; } 

    public CollectionGuidModel ToCollectionModel()
    {
      return new CollectionGuidModel
      {
        Id = Id,
        Value = Value
      };
    }
  }
}
