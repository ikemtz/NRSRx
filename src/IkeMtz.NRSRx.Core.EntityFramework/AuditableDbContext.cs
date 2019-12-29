using IkeMtz.NRSRx.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public class AuditableDbContext : DbContext, IAuditableDbContext
  {
    protected  IHttpContextAccessor HttpContextAccessor { get; }
    public AuditableDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
      HttpContextAccessor = httpContextAccessor;
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
      var entities = ChangeTracker.Entries().Where(x => x.Entity is ICalculateable);

      foreach (var calculateable in entities
          .Select(t => t.Entity as ICalculateable)
          .Where(t => t != null))
      {
        calculateable.CalculateValues();
      }
      entities = ChangeTracker.Entries().Where(x => x.Entity is IAggregratedByParents);

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
      var entities = ChangeTracker.Entries().Where(x => x.Entity is IAuditable && (x.State == EntityState.Added));
      var currentUsername = HttpContextAccessor.HttpContext.User.Identity.Name;

      foreach (var auditable in entities
          .Select(t => t.Entity as IAuditable)
          .Where(t => t != null))
      {
        auditable.CreatedOnUtc = DateTime.UtcNow;
        auditable.CreatedBy = currentUsername;
      }
      foreach (var disableable in entities
          .Select(t => t.Entity as IDisableable)
          .Where(t => t != null))
      {
        disableable.IsEnabled = true;
      }
    }

    private void UpdateAuditables()
    {
      var entities = ChangeTracker.Entries().Where(x => x.Entity is IAuditable && (x.State == EntityState.Modified));
      var currentUsername = HttpContextAccessor.HttpContext.User.Identity.Name;

      foreach (var auditable in entities
         .Select(t => t.Entity as IAuditable)
         .Where(t => t != null))
      {
        auditable.UpdatedOnUtc = DateTime.UtcNow;
        auditable.UpdatedBy = currentUsername;
      }
    }
  }
}
