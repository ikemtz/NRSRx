using System;
using IkeMtz.Samples.Models;

namespace IkeMtz.NRSRx.Tests
{
  public static partial class Factories
  {
    public static Item ItemFactory()
    {
      return new Item()
      {
        Id = Guid.NewGuid(),
        Value = Guid.NewGuid().ToString()[..6],
        TenantId = "NRSRX",
      };
    }
  }
}
