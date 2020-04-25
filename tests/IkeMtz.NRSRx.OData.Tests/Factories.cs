using System;
using IkeMtz.Samples.OData.Models;

namespace IkeMtz.NRSRx.OData.Tests
{
  public static partial class Factories
  {
    public static Item ItemFactory()
    {
      return new Item()
      {
        Id = Guid.NewGuid(),
        Value = Guid.NewGuid().ToString().Substring(0, 6)
      };
    }
  }
}
