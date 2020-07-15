using System;
using IkeMtz.Samples.WebApi.Models;

namespace IkeMtz.NRSRx.Tests
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
