using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public static ODataEnvelope<Entity, IdentityType> Create(IEnumerable<Entity> t)
    {
      return new ODataEnvelope<Entity, IdentityType>()
      {
        Count = t.Count(),
        Value = t
      };
    }
  }
}
