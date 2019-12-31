using System.Threading;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public interface IAuditableDbContext
  {
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
  }
}
