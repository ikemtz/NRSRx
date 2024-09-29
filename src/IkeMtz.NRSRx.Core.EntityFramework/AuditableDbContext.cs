using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  /// <summary>
  /// Represents a DbContext that supports auditing of entities.
  /// </summary>
  public class AuditableDbContext(DbContextOptions options, ICurrentUserProvider currentUserProvider) : DbContext(options), IAuditableDbContext
  {
    /// <summary>
    /// Gets or sets the current user provider.
    /// </summary>
    public ICurrentUserProvider CurrentUserProvider { get; set; } = currentUserProvider;

    /// <summary>
    /// Adds an entity to the context asynchronously.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity entry.</returns>
    public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
    {
      if (entity is ICalculateable)
      {
        (entity as ICalculateable).CalculateValues();
      }
      if (entity is IAuditable)
      {
        OnIAuditableCreate(entity as IAuditable);
      }
      return base.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Called when an auditable entity is created.
    /// </summary>
    /// <param name="auditable">The auditable entity.</param>
    public virtual void OnIAuditableCreate(IAuditable auditable)
    {
      auditable.CreatedOnUtc = auditable.CreatedOnUtc.Year != 1 ? auditable.CreatedOnUtc : DateTime.UtcNow;
#pragma warning disable CS8601 // Possible null reference assignment.
      auditable.CreatedBy = CurrentUserProvider.GetCurrentUserId();
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    /// <summary>
    /// Called when an auditable entity is updated.
    /// </summary>
    /// <param name="auditable">The auditable entity.</param>
    public virtual void OnIAuditableUpdate(IAuditable auditable)
    {
      auditable.UpdatedOnUtc = DateTime.UtcNow;
      auditable.UpdatedBy = CurrentUserProvider.GetCurrentUserId();
      auditable.UpdateCount = (auditable.UpdateCount ?? 0) + 1;
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether to accept all changes on success.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
      CalculateValues();
      AddAuditables();
      UpdateAuditables();

      var recordCount = base.SaveChanges(acceptAllChangesOnSuccess);
      return recordCount;
    }

    /// <summary>
    /// Saves all changes made in this context to the database and logs the result.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether to accept all changes on success.</param>
    /// <param name="logger">The logger to use.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public int SaveChanges(bool acceptAllChangesOnSuccess, ILogger logger)
    {
      var rowCount = this.SaveChanges(acceptAllChangesOnSuccess);
      logger?.LogInformation("Save changes completed successfully, affected row count: {rowCount}", rowCount);
      return rowCount;
    }

    /// <summary>
    /// Saves all changes made in this context to the database and logs the result.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public int SaveChanges(ILogger logger)
    {
      return SaveChanges(true, logger);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    public override int SaveChanges()
      => SaveChanges(acceptAllChangesOnSuccess: true);

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
      => SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether to accept all changes on success.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
      CalculateValues();
      AddAuditables();
      UpdateAuditables();
      var recordCount = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
      return recordCount;
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database and logs the result.
    /// </summary>
    /// <param name="acceptAllChangesOnSuccess">Indicates whether to accept all changes on success.</param>
    /// <param name="logger">The logger to use.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, ILogger logger)
    {
      var rowCount = await SaveChangesAsync(acceptAllChangesOnSuccess);
      logger?.LogInformation("Save changes completed successfully, affected row count: {rowCount}", rowCount);
      return rowCount;
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database and logs the result.
    /// </summary>
    /// <param name="logger">The logger to use.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public Task<int> SaveChangesAsync(ILogger logger)
    {
      return SaveChangesAsync(true, logger);
    }

    /// <summary>
    /// Calculates values for entities that implement <see cref="ICalculateable"/>.
    /// </summary>
    public virtual void CalculateValues()
    {
      var entities = base.ChangeTracker.Entries().Where(x => x.Entity is ICalculateable);

      foreach (var calculateable in entities
          .Select(t => t.Entity as ICalculateable)
          .Where(t => t != null))
      {
        calculateable.CalculateValues();
      }
    }

    /// <summary>
    /// Adds audit information for entities that implement <see cref="IAuditable"/> and are in the Added state.
    /// </summary>
    public virtual void AddAuditables()
    {
      var entities = base.ChangeTracker.Entries().Where(x => x.Entity is IAuditable && (x.State == EntityState.Added));

      foreach (var auditable in entities
          .Select(t => t.Entity as IAuditable)
          .Where(t => t != null))
      {
        OnIAuditableCreate(auditable);
      }
    }

    /// <summary>
    /// Updates audit information for entities that implement <see cref="IAuditable"/> and are in the Modified state.
    /// </summary>
    public virtual void UpdateAuditables()
    {
      var entities = base.ChangeTracker.Entries().Where(x => x.Entity is IAuditable && (x.State == EntityState.Modified));

      foreach (var auditable in entities
         .Select(t => t.Entity as IAuditable)
         .Where(t => t != null))
      {
        OnIAuditableUpdate(auditable);
      }
    }
  }
}
