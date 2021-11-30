using System;
using System.Collections.Generic;
using System.Linq;
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

    public static IEdmModel ODataConventionModelFactory(Action<ODataConventionModelBuilder> action)
    {
      var builder = new ODataConventionModelBuilder();
      action(builder);
      _ = builder.EnableLowerCamelCase();
      return builder.GetEdmModel();
    }
    public IEnumerable<ApiVersionDescription> GetODataVersions()
    {
      return EdmModels.Select(t => t.Key);
    }
  }
}

