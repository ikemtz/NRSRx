using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OData.Edm;

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

    public IEnumerable<ApiVersionDescription> GetODataVersions()
    {
      return EdmModels.Select(t => t.Key);
    }
  }
}

