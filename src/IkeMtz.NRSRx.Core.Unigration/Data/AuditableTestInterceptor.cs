using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IkeMtz.NRSRx.Core.Unigration.Data
{
  public class AuditableTestInterceptor : ISaveChangesInterceptor
  {
    public ICurrentUserProvider CurrentUserProvider { get; }
    public AuditableTestInterceptor(ICurrentUserProvider currentUserProvider)
    {
      CurrentUserProvider = currentUserProvider;
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
      SetAuditables(eventData);
      return result;
    }

    public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
      SetAuditables(eventData);
      return ValueTask.FromResult(result);
    }

    public void SetAuditables(DbContextEventData eventData)
    {
      var entries = eventData.Context.ChangeTracker.Entries()
        .Where(x => x.Entity is IAuditable);
      entries
        .Where(x => x.State == EntityState.Added)
        .Select(x => x.Entity)
        .Cast<IAuditable>()
        .ToList()
        .ForEach(x =>
        {
          x.CreatedOnUtc = x.CreatedOnUtc != DateTime.MinValue ? x.CreatedOnUtc : DateTime.UtcNow;
          x.CreatedBy = CurrentUserProvider?.GetCurrentUserId();
        });
      entries
        .Where(x => x.State == EntityState.Modified)
        .Select(x => x.Entity)
        .Cast<IAuditable>()
        .ToList()
        .ForEach(x =>
        {
          x.UpdatedOnUtc = x.UpdatedOnUtc != DateTime.MinValue ? x.UpdatedOnUtc : DateTime.UtcNow;
          x.UpdatedBy = CurrentUserProvider?.GetCurrentUserId();
          x.UpdateCount = (x.UpdateCount ?? 0) + 1;
        });
    }
  }
}
