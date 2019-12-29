using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData.Edm;
using System;
using System.Diagnostics.CodeAnalysis;

namespace IkeMtz.NRSRx.Core.OData
{
  public class NrsrxODataSerializerProvider : DefaultODataSerializerProvider
  {
    private readonly NrsrxODataSerializer _entityTypeSerializer;
    public NrsrxODataSerializerProvider(IServiceProvider rootContainer) : base(rootContainer)
    {
      _entityTypeSerializer = new NrsrxODataSerializer(this);
    }

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods",
      Justification = "The IEdmTypeReference is provided by the framework.")]
    public override ODataEdmTypeSerializer GetEdmTypeSerializer(IEdmTypeReference edmType)
    {
      if (edmType.Definition.TypeKind == EdmTypeKind.Entity || edmType.Definition.TypeKind == EdmTypeKind.Complex)
        return _entityTypeSerializer;
      else
        return base.GetEdmTypeSerializer(edmType);
    }
  }
}
