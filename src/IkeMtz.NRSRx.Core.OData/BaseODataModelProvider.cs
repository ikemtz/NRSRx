using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace IkeMtz.NRSRx.Core.OData
{
  /// <summary>
  /// Abstract base class for providing OData models and version descriptions.
  /// </summary>
  public abstract class BaseODataModelProvider : IODataVersionProvider
  {
    private static IDictionary<ApiVersionDescription, IEdmModel>? _cached;

    /// <summary>
    /// Gets the cached OData EDM models.
    /// </summary>
    public IDictionary<ApiVersionDescription, IEdmModel> EdmModels
    {
      get
      {
        _cached ??= GetModels();
        return _cached;
      }
    }

    /// <summary>
    /// Gets the OData EDM models.
    /// </summary>
    /// <returns>A dictionary of <see cref="ApiVersionDescription"/> and <see cref="IEdmModel"/>.</returns>
    public abstract IDictionary<ApiVersionDescription, IEdmModel> GetModels();

    /// <summary>
    /// Creates an OData EDM model using the specified action.
    /// </summary>
    /// <param name="action">The action to configure the <see cref="ODataConventionModelBuilder"/>.</param>
    /// <returns>An instance of <see cref="IEdmModel"/>.</returns>
    public static IEdmModel ODataEntityModelFactory(Action<ODataConventionModelBuilder> action)
    {
      var builder = new ODataConventionModelBuilder();
      action(builder);
      return builder
        .EnableLowerCamelCase()
        .GetEdmModel();
    }

    /// <summary>
    /// Creates an API version description.
    /// </summary>
    /// <param name="MajorVersion">The major version number.</param>
    /// <param name="MinorVersion">The minor version number.</param>
    /// <param name="IsDeprecated">Indicates whether the version is deprecated.</param>
    /// <returns>An instance of <see cref="ApiVersionDescription"/>.</returns>
    public static ApiVersionDescription ApiVersionFactory(int MajorVersion, int MinorVersion, bool IsDeprecated = false) =>
      new(
        new ApiVersion(MajorVersion, MinorVersion),
          $"v{MajorVersion}.{MinorVersion}".Replace(".0", ""),
          IsDeprecated);

    /// <summary>
    /// Gets the collection of OData version descriptions.
    /// </summary>
    /// <returns>A collection of <see cref="ApiVersionDescription"/> objects.</returns>
    public IEnumerable<ApiVersionDescription> GetODataVersions()
    {
      return EdmModels.Select(t => t.Key);
    }
  }
}

