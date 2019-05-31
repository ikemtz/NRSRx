using IkeMtz.NRSRx.Core.Models.Validation;
using System;

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
