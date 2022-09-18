using IkeMtz.NRSRx.Core.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class TestAuditableDbContext : AuditableDbContext
  {
    public TestAuditableDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options, httpContextAccessor)
    {
    }
    public DbSet<MyIntModel> MyModel { get; set; }
    public DbSet<CollectionGuidModel> CollectionGuidModels { get; set; }
    public DbSet<CollectionIntModel> CollectionIntModels { get; set; }
  }

  public class TestDbContext : DbContext
  {
    public TestDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<MyIntModel> MyModel { get; set; }
    public DbSet<CollectionGuidModel> CollectionGuidModels { get; set; }
    public DbSet<CollectionIntModel> CollectionIntModels { get; set; }
  }
}
