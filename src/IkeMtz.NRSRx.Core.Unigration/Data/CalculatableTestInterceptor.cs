using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IkeMtz.NRSRx.Core.Unigration.Data
{
  /// <summary>
  /// Interceptor for handling calculatable entities during save changes operations in the DbContext.
  /// </summary>
  public class CalculatableTestInterceptor : ISaveChangesInterceptor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="CalculatableTestInterceptor"/> class.
    /// </summary>
    public CalculatableTestInterceptor()
    {
    }

    /// <summary>
    /// Called when save changes operation fails.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    public void SaveChangesFailed(DbContextErrorEventData eventData)
    {
    }

    /// <summary>
    /// Called asynchronously when save changes operation fails.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A completed task.</returns>
    public Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
      return Task.CompletedTask;
    }

    /// <summary>
    /// Called after save changes operation completes.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The result of the save changes operation.</param>
    /// <returns>The result of the save changes operation.</returns>
    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
      return result;
    }

    /// <summary>
    /// Called asynchronously after save changes operation completes.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The result of the save changes operation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result of the save changes operation.</returns>
    public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
      return ValueTask.FromResult(result);
    }

    /// <summary>
    /// Called before save changes operation.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The interception result.</param>
    /// <returns>The interception result.</returns>
    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
      SetCalculatables(eventData);
      return result;
    }

    /// <summary>
    /// Called asynchronously before save changes operation.
    /// </summary>
    /// <param name="eventData">The event data.</param>
    /// <param name="result">The interception result.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The interception result.</returns>
    public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
      SetCalculatables(eventData);
      return ValueTask.FromResult(result);
    }

    /// <summary>
    /// Sets the calculatable fields for calculatable entities.
    /// </summary>
    /// <param name="eventData">The event data.</param>
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
