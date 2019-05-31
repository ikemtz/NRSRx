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
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public AuditableDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
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
        }

        private void AddAuditables()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is IAuditable && (x.State == EntityState.Added));
            var currentUsername = _httpContextAccessor.HttpContext.User.Identity.Name;

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
            var currentUsername = _httpContextAccessor.HttpContext.User.Identity.Name;

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
