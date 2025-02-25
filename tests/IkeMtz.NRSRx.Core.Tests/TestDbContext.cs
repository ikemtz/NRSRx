using Microsoft.EntityFrameworkCore;

namespace IkeMtz.NRSRx.Core.Tests
{
  public class TestDbContext : DbContext
  {
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }
    public DbSet<MyIntModel> MyModel { get; set; }
    public DbSet<CollectionGuidModel> CollectionGuidModels { get; set; }
    public DbSet<CollectionIntModel> CollectionIntModels { get; set; }
  }
}
