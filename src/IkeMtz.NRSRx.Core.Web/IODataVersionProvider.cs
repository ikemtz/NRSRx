using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace IkeMtz.NRSRx.Core.OData
{
  /// <summary>
  /// Defines a contract for providing OData version descriptions.
  /// </summary>
  public interface IODataVersionProvider
  {
    /// <summary>
    /// Gets the collection of OData version descriptions.
    /// </summary>
    /// <returns>A collection of <see cref="ApiVersionDescription"/> objects.</returns>
    IEnumerable<ApiVersionDescription> GetODataVersions();
  }
}
