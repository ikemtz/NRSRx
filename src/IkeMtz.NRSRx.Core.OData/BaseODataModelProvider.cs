using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace IkeMtz.NRSRx.Core.OData
{
  public abstract class BaseODataModelProvider : IODataVersionProvider
  {
    private static IDictionary<ApiVersionDescription, IEdmModel> _cached;
    public IDictionary<ApiVersionDescription, IEdmModel> EdmModels
    {
      get
      {
        if (_cached == null)
        {
          _cached = GetModels();
        }
        return _cached;
      }
    }

    public abstract IDictionary<ApiVersionDescription, IEdmModel> GetModels();

    public static IEdmModel ODataEntityModelFactory(Action<ODataConventionModelBuilder> action)
    {
      var builder = new ODataConventionModelBuilder();
      action(builder);
      return builder
        .EnableLowerCamelCase()
        .GetEdmModel();
    }
    public static ApiVersionDescription ApiVersionFactory(int MajorVersion, int MinorVersion, bool IsDeprecated = false) =>
      new(
        new ApiVersion(MajorVersion, MinorVersion),
          $"v{MajorVersion}.{MinorVersion}".Replace(".0", ""),
          IsDeprecated);

    public IEnumerable<ApiVersionDescription> GetODataVersions()
    {
      return EdmModels.Select(t => t.Key);
    }
  }
}

