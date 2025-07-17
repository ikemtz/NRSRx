using System;

namespace IkeMtz.NRSRx.Core.Models
{
  /// <summary>
  /// Represents an OData envelope for entities with a <see cref="Guid"/> identifier.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  public class ODataEnvelope<TEntity> : ODataEnvelope<TEntity, Guid>
         where TEntity : class, IIdentifiable, new()
  {
  }

  /// <summary>
  /// Represents an OData envelope for entities with a specified identifier type.
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  /// <typeparam name="TIdentityType">The type of the identifier.</typeparam>
  public class ODataEnvelope<TEntity, TIdentityType>
    : CompositeKeyODataEnvelope<TEntity>
    where TIdentityType : IComparable
    where TEntity : class, IIdentifiable<TIdentityType>, new()
  {
  }
}
