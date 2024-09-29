using System;
using System.Collections.Immutable;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData;
using Microsoft.OData.Edm;

namespace IkeMtz.NRSRx.Core.OData
{
  /// <summary>
  /// Provides a custom OData resource serializer that omits default values for date and numeric types.
  /// </summary>
  /// <remarks>
  /// Initializes a new instance of the <see cref="NrsrxODataSerializer"/> class.
  /// </remarks>
  /// <param name="serializerProvider">The serializer provider.</param>
  public class NrsrxODataSerializer(ODataSerializerProvider serializerProvider) : ODataResourceSerializer(serializerProvider)
  {
    private static ImmutableList<Type> DateTypes => [typeof(DateTime), typeof(DateTimeOffset)
];

    private static ImmutableList<Type> NumericTypes =>
    [
      typeof(short),
      typeof(int),
      typeof(long),
      typeof(decimal),
      typeof(double),
      typeof(float),
      typeof(long),
      typeof(short),
      typeof(ulong),
      typeof(ushort)
,
    ];

    /// <summary>
    /// Creates an OData structural property, omitting properties with default values for date and numeric types.
    /// </summary>
    /// <param name="structuralProperty">The EDM structural property.</param>
    /// <param name="resourceContext">The resource context.</param>
    /// <returns>An instance of <see cref="ODataProperty"/> or null if the property should be omitted.</returns>
    public override ODataProperty? CreateStructuralProperty(IEdmStructuralProperty structuralProperty, ResourceContext resourceContext)
    {
      var property = base.CreateStructuralProperty(structuralProperty, resourceContext);
      if (property.Value == null)
      {
        return null;
      }
      else if (DateTypes.Contains(property.Value.GetType()) && ((dynamic)property.Value == default(DateTime)))
      {
        return null;
      }
      else if (NumericTypes.Contains(property.Value.GetType()) && (dynamic)property.Value == 0)
      {
        return null;
      }
      else
        return property;
    }
  }
}
