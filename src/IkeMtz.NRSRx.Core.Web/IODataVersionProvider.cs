using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace IkeMtz.NRSRx.Core.OData
{
  public interface IODataVersionProvider
  {
    IEnumerable<ApiVersionDescription> GetODataVersions();
  }
}
