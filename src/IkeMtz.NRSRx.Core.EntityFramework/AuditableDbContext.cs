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
    protected IHttpContextAccessor HttpContextAccessor { get; set; }
    public AuditableDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
      HttpContextAccessor = httpContextAccessor;
    }

    public void SetHttpContext(IHttpContextAccessor httpContextAccessor)
    {
      this.HttpContextAccessor = httpContextAccessor;
    }
    public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
    {
      if (entity is IAuditable)
      {
        OnIAuditableCreate(entity as IAuditable);
      }
      if (entity is ICalculateable)
      {
        (entity as ICalculateable).CalculateValues();
      }
      return base.AddAsync(entity, cancellationToken);
    }
    public virtual void OnIAuditableCreate(IAuditable auditable)
    {
      if (string.IsNullOrWhiteSpace(HttpContextAccessor?.HttpContext?.User?.Identity?.Name))
      {
        throw new AuditableInvalidUserException();
      }
      auditable.CreatedOnUtc = auditable.CreatedOnUtc.Year != 1 ? auditable.CreatedOnUtc : DateTime.UtcNow;
      auditable.CreatedBy = HttpContextAccessor.HttpContext.User.Identity.Name;
    }
    public virtual void OnIAuditableUpdate(IAuditable auditable)
    {
      if (string.IsNullOrWhiteSpace(HttpContextAccessor?.HttpContext?.User?.Identity?.Name))
      {
        throw new AuditableInvalidUserException();
      }
      auditable.UpdatedOnUtc = DateTime.UtcNow;
      auditable.UpdatedBy = HttpContextAccessor.HttpContext.User.Identity.Name;
    }
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
      CalculateValues();
      AddAuditables();
      UpdateAuditables();
      return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override int SaveChanges()
      => SaveChanges(acceptAllChangesOnSuccess: true);
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
      => SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
      CalculateValues();
      AddAuditables();
      UpdateAuditables();
      return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
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
