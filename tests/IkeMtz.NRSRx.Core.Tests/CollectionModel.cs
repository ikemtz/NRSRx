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

    public CollectionModel Clone()
    {
      return new CollectionModel
      {
        Id = Id,
        Value = Value
      };
    }
  }

  public class CollectionModelDto : IInsertableDto
  {
    public CollectionModelDto()
    {
      Id = Guid.NewGuid();
      Value = Guid.NewGuid().ToString();
    }
    public Guid? Id { get; set; }
    public string Value { get; set; }
    public CollectionModel ToCollectionModel()
    {
      return new CollectionModel
      {
        Id = Id.GetValueOrDefault(),
        Value = Value
      };
    }
  }
}
