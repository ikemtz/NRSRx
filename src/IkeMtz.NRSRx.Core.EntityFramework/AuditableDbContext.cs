using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public class AuditableDbContext : DbContext, IAuditableDbContext
  {
    public ICurrentUserProvider CurrentUserProvider { get; set; }

    public ILogger<AuditableDbContext> Logger { get; set; }
    public AuditableDbContext(DbContextOptions options, ICurrentUserProvider currentUserProvider)
        : base(options)
    {
      CurrentUserProvider = currentUserProvider;
      Logger = this.GetService<ILogger<AuditableDbContext>>();
    }

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
    public virtual void OnIAuditableCreate(IAuditable auditable)
    {
      auditable.CreatedOnUtc = auditable.CreatedOnUtc.Year != 1 ? auditable.CreatedOnUtc : DateTime.UtcNow;
#pragma warning disable CS8601 // Possible null reference assignment.
      auditable.CreatedBy = CurrentUserProvider.GetCurrentUserId();
#pragma warning restore CS8601 // Possible null reference assignment.
    }
    public virtual void OnIAuditableUpdate(IAuditable auditable)
    {
      auditable.UpdatedOnUtc = DateTime.UtcNow;
      auditable.UpdatedBy = CurrentUserProvider.GetCurrentUserId();
      auditable.UpdateCount = (auditable.UpdateCount ?? 0) + 1;
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
      CalculateValues();
      AddAuditables();
      UpdateAuditables();

      var recordCount = base.SaveChanges(acceptAllChangesOnSuccess);
      Logger?.LogInformation("State entries written to database: {recordCount}.", recordCount);
      return recordCount;
    }
    public override int SaveChanges()
      => SaveChanges(acceptAllChangesOnSuccess: true);
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
      => SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
      CalculateValues();
      AddAuditables();
      UpdateAuditables();
      var recordCount = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
      Logger?.LogInformation("State entries written to database: {recordCount}.", recordCount);
      return recordCount;
    }
    public virtual void CalculateValues()
    {
      var entities = base.ChangeTracker.Entries().Where(x => x.Entity is ICalculateable);

      foreach (var calculateable in entities
          .Select(t => t.Entity as ICalculateable)
          .Where(t => t != null))
      {
        calculateable.CalculateValues();
      }
      entities = base.ChangeTracker.Entries().Where(x => x.Entity is IAggregratedByParents);

      foreach (var aggretable in entities
          .Select(t => t.Entity as IAggregratedByParents)
          .Where(t => t != null && t.Parents != null))
      {
        foreach (var parent in aggretable.Parents.Where(t => t != null))
        {
          parent.CalculateValues();
        }
      }
    }

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
