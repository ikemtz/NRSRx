using System.Collections.Generic;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Defines a contract for providing API version definitions.
  /// </summary>
  public interface IApiVersionDefinitions
  {
    /// <summary>
    /// Gets the collection of API versions.
    /// </summary>
    IEnumerable<string> Versions { get; }
  }
}
