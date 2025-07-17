using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Models
{
  public class CompositeKeyODataEnvelope<TEntity>
    where TEntity : class, new()
  {
    /// <summary>
    /// Gets or sets the collection of entities.
    /// </summary>
    [JsonProperty("value")]
    public IEnumerable<TEntity> Value { get; set; }

    /// <summary>
    /// Gets or sets the count of entities.
    /// </summary>
    [JsonProperty("@odata.count")]
    public int? Count { get; set; }

    /// <summary>
    /// Creates an OData envelope from the specified collection of entities.
    /// </summary>
    /// <param name="t">The collection of entities.</param>
    /// <returns>An OData envelope containing the entities.</returns>
    public static CompositeKeyODataEnvelope<TEntity> Create(IEnumerable<TEntity> t)
    {
      return new CompositeKeyODataEnvelope<TEntity>()
      {
        Count = t.Count(),
        Value = t
      };
    }
  }
}
