using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public class AuditableDbContext : DbContext, IAuditableDbContext
  {
    protected IHttpContextAccessor HttpContextAccessor { get; }
    public AuditableDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
      HttpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default)
    {
      if (entity is IAuditable)
      {
        handleIAuditableCreate(entity as IAuditable);
      }
      if (entity is ICalculateable)
      {
        (entity as ICalculateable).CalculateValues();
      }
      return base.AddAsync(entity, cancellationToken);
    }

    public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
    {
      if (entity is IAuditable)
      {
        handleIAuditableCreate(entity as IAuditable);
      }
      if (entity is ICalculateable)
      {
        (entity as ICalculateable).CalculateValues();
      }
      return base.AddAsync(entity, cancellationToken);
    }
    private void handleIAuditableCreate(IAuditable auditable)
    {
      auditable.CreatedOnUtc = auditable.CreatedOnUtc.Year != 1 ? auditable.CreatedOnUtc : DateTime.UtcNow;
      auditable.CreatedBy ??= HttpContextAccessor.HttpContext.User.Identity.Name;
    }
    private void handleIAuditableUpdate(IAuditable auditable)
    {
      auditable.UpdatedOnUtc = DateTime.UtcNow;
      auditable.UpdatedBy = HttpContextAccessor.HttpContext.User.Identity.Name;
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
      CalculateValues();
      AddAuditables();
      UpdateAuditables();
      return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    private void CalculateValues()
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

    private void AddAuditables()
    {
      var entities = base.ChangeTracker.Entries().Where(x => x.Entity is IAuditable && (x.State == EntityState.Added));

      foreach (var auditable in entities
          .Select(t => t.Entity as IAuditable)
          .Where(t => t != null))
      {
        handleIAuditableCreate(auditable);
      }
    }

    private void UpdateAuditables()
    {
      var entities = base.ChangeTracker.Entries().Where(x => x.Entity is IAuditable && (x.State == EntityState.Modified));

      foreach (var auditable in entities
         .Select(t => t.Entity as IAuditable)
         .Where(t => t != null))
      {
        handleIAuditableUpdate(auditable);
      }
    }
  }
}
