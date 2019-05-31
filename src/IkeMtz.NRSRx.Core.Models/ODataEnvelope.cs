using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IkeMtz.NRSRx.Core.Models
{
    public class ODataEnvelope<Entity> : ODataEnvelope<Entity, Guid>
         where Entity : class, IIdentifiable, new()
    {
    }

    public class ODataEnvelope<Entity, IdentityType> where Entity : class, IIdentifiable<IdentityType>, new()
    {
        [JsonProperty("value")]
        public IEnumerable<Entity> Value { get; set; }

        [JsonProperty("@odata.count")]
         public int Count { get; set; }
    }
}
