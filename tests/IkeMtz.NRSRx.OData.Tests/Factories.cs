using System;
using IkeMtz.Samples.OData.Models;

namespace IkeMtz.NRSRx.OData.Tests
{
  public static partial class Factories
  {
    public static Item ItemFactory()
    {
      var item = new Item()
      {
        Id = Guid.NewGuid(),
        Value = Guid.NewGuid().ToString().Substring(0, 6),
        TenantId = "NRSRX",
        CreatedBy = "Factory",
        CreatedOnUtc = DateTime.UtcNow
      };
      item.SubItemAs.Add(new SubItemA
      {
        Id = Guid.NewGuid(),
        ValueA = Guid.NewGuid().ToString().Substring(0, 6)
      });
      item.SubItemBs.Add(new SubItemB
      {
        Id = Guid.NewGuid(),
        ValueB = Guid.NewGuid().ToString().Substring(0, 6)
      });
      item.SubItemCs.Add(new SubItemC
      {
        Id = Guid.NewGuid(),
        ValueC = Guid.NewGuid().ToString().Substring(0, 6)
      });

      return item;
    }
  }
}
