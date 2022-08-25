using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IkeMtz.NRSRx.Core.Unigration.Data
{
  public class CalculatableTestInterceptor : ISaveChangesInterceptor
  {
    public CalculatableTestInterceptor()
    {
    }

    public void SaveChangesFailed(DbContextErrorEventData eventData)
    {
    }

    public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
      return Task.CompletedTask;
    }

    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
      return result;
    }

    public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
      return ValueTask.FromResult(result);
    }

    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
      SetCalculatables(eventData);
      return result;
    }

    public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
      SetCalculatables(eventData);
      return ValueTask.FromResult(result);
    }

    public void SetCalculatables(DbContextEventData eventData)
    {
      var entries = eventData.Context.ChangeTracker.Entries()
        .Where(x => x.Entity is ICalculateable);
      entries
        .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)
        .Select(x => x.Entity)
        .Cast<ICalculateable>()
        .ToList()
        .ForEach(x => x.CalculateValues());
    }
  }
}
