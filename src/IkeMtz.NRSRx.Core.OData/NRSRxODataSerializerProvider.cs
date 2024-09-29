using System;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData.Edm;

namespace IkeMtz.NRSRx.Core.OData
{
  /// <summary>
  /// Provides a custom OData serializer provider that uses <see cref="NrsrxODataSerializer"/> for entity and complex types.
  /// </summary>
  public class NrsrxODataSerializerProvider : ODataSerializerProvider
  {
    private readonly NrsrxODataSerializer _entityTypeSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="NrsrxODataSerializerProvider"/> class.
    /// </summary>
    /// <param name="rootContainer">The root service provider.</param>
    public NrsrxODataSerializerProvider(IServiceProvider rootContainer) : base(rootContainer)
    {
      _entityTypeSerializer = new NrsrxODataSerializer(this);
    }

    /// <summary>
    /// Gets the appropriate serializer for the given EDM type.
    /// </summary>
    /// <param name="edmType">The EDM type reference.</param>
    /// <returns>An instance of <see cref="IODataEdmTypeSerializer"/>.</returns>
    public override IODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
    {
      if (edmType.Definition.TypeKind == EdmTypeKind.Entity || edmType.Definition.TypeKind == EdmTypeKind.Complex)
        return _entityTypeSerializer;
      else
        return base.GetEdmTypeSerializer(edmType);
    }
  }
}
