using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace IkeMtz.NRSRx.Core.OData
{
  public abstract class BaseODataModelProvider
  {
    private static IDictionary<string, IEdmModel> _cached;
    internal IDictionary<string, IEdmModel> GetEdmModel()
    {
      if (_cached == null)
      {
        _cached = GetModels();
      }
      return _cached;
    }

    public abstract IDictionary<string, IEdmModel> GetModels();
  }
}

