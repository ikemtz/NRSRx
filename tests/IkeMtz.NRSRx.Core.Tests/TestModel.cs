using System;
using IkeMtz.NRSRx.Core.Models.Validation;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class TestModel
  {
    [RequiredNonDefault]
    public Guid TestGuid { get; set; }

    [RequiredNonEmpty]
    public string[] strings { get; set; }
  }
}
