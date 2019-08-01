using System.Collections.Generic;

namespace IkeMtz.NRSRx.Core.Models
{
  public interface IAggregratedByParents
  {
    IEnumerable<ICalculateable> Parents { get; }
  }
}
