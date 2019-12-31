using System.Collections.Generic;

namespace IkeMtz.NRSRx.Core.Web
{
  public interface IApiVersionDefinitions
  {
    IEnumerable<string> Versions { get; }
  }
}
