using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>  
  /// Represents an OData envelope that extends <see cref="ODataEnvelope{TEntity}"/>  
  /// to support entities with a specified identifier type.  
  /// </summary>  
  /// <typeparam name="TEntity">The type of the entity.</typeparam>  
  /// <typeparam name="TIdentityType">The type of the identifier.</typeparam>  
  public class ODataEnvelope<TEntity, TIdentityType>
      : ODataEnvelope<TEntity>
      where TIdentityType : IComparable
      where TEntity : class, IIdentifiable<TIdentityType>, new()
  {
  }

  /// <summary>  
  /// Represents the base class for OData envelopes that do not require entities to implement <see cref="IIdentifiable"/>.  
  /// This class is used to encapsulate a collection of entities without identifier constraints.  
  /// <typeparam name="TEntity">The type of the entity.</typeparam>  
  public class ODataEnvelope<TEntity>
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
    public static ODataEnvelope<TEntity> Create(IEnumerable<TEntity> t)
    {
      return new ODataEnvelope<TEntity>()
      {
        Count = t.Count(),
        Value = t
      };
    }
  }
}
