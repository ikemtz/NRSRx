using System;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData.Edm;

namespace IkeMtz.NRSRx.Core.OData
{
  public class NrsrxODataSerializerProvider : ODataSerializerProvider
  {
    private readonly NrsrxODataSerializer _entityTypeSerializer;
    public NrsrxODataSerializerProvider(IServiceProvider rootContainer) : base(rootContainer)
    {
      _entityTypeSerializer = new NrsrxODataSerializer(this);
    }

    public override IODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
    {
      if (edmType.Definition.TypeKind == EdmTypeKind.Entity || edmType.Definition.TypeKind == EdmTypeKind.Complex)
        return _entityTypeSerializer;
      else
        return base.GetEdmTypeSerializer(edmType);
    }
  }
}
