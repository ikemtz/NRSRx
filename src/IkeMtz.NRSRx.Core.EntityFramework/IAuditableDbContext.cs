using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  /// <summary>
  /// Interface for an auditable database context.
  /// </summary>
  public interface IAuditableDbContext
  {
    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges"/> is called after the changes have been sent successfully to the database.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks the given entity as <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Deleted"/> such that it will be deleted from the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges"/> is called.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <returns>The <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry"/> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
    EntityEntry Remove(object entity);

    /// <summary>
    /// Marks the given entity as <see cref="F:Microsoft.EntityFrameworkCore.EntityState.Added"/> such that it will be inserted into the database when <see cref="M:Microsoft.EntityFrameworkCore.DbContext.SaveChanges"/> is called.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The <see cref="T:Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry"/> for the entity. The entry provides access to change tracking information and operations for the entity.</returns>
    EntityEntry Add(object entity);
  }
}
