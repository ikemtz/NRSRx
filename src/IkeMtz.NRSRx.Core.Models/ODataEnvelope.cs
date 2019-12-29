using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Models
{
  public class ODataEnvelope<TEntity> : ODataEnvelope<TEntity, Guid>
       where TEntity : class, IIdentifiable, new()
  {
  }

  public class ODataEnvelope<TEntity, TIdentityType> where TEntity : class, IIdentifiable<TIdentityType>, new()
  {
    [JsonProperty("value")]
    public IEnumerable<TEntity> Value { get; set; }

    [JsonProperty("@odata.count")]
    public int Count { get; set; }

    public static ODataEnvelope<TEntity, TIdentityType> Create(IEnumerable<TEntity> t)
    {
      return new ODataEnvelope<TEntity, TIdentityType>()
      {
        Count = t.Count(),
        Value = t
      };
    }
  }
}
