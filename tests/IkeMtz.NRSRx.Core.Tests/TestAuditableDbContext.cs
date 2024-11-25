using IkeMtz.NRSRx.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class TestAuditableDbContext : AuditableDbContext
  {
    public TestAuditableDbContext(DbContextOptions options, ICurrentUserProvider currentUserProvider) : base(options, currentUserProvider)
    {
    }
    public DbSet<MyIntModel> MyModel { get; set; }
    public DbSet<CollectionGuidModel> CollectionGuidModels { get; set; }
    public DbSet<CollectionIntModel> CollectionIntModels { get; set; }
  }
}
