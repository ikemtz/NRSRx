using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class MockAuditableDbContextFactoryTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public async Task MockAuditableContextFactoryTest()
    {
      var modelA = new MyModel();
      var modelB = new MyModel();
      var fac = new DbContextFactory();
      using var ctx = fac.CreateInMemoryAuditableDbContext<TestAuditableContext>(TestContext);
      _ = ctx.MyModel.Add(modelA);
      _ = ctx.MyModel.Add(modelB);
      _ = await ctx.SaveChangesAsync();
      Assert.AreEqual(2, await ctx.MyModel.CountAsync());
      modelA.UpdatedBy = "Not Me";
      _ = await ctx.SaveChangesAsync();
      Assert.AreEqual("NRSRx Test User", modelA.UpdatedBy);
      Assert.IsNull(modelB.UpdatedOnUtc);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task MockAuditableContextAddAsyncTest()
    {
      var modelA = new MyModel();
      var modelB = new MyModel();
      var fac = new DbContextFactory();
      using var ctx = fac.CreateInMemoryAuditableDbContext<TestAuditableContext>(TestContext);
      var dbContextObjA = await ctx.AddAsync(modelA);
      var dbContextObjB = await ctx.AddAsync<MyModel>(modelB);
      Assert.AreEqual(1, dbContextObjA.Entity.Id);
      Assert.AreEqual(2, dbContextObjB.Entity.Id);
      Assert.IsNotNull(dbContextObjA.Entity.CalculatedValue);
      Assert.IsNotNull(dbContextObjB.Entity.CalculatedValue);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public async Task MockDbContextFactoryTest()
    {
      var modelA = new MyModel();
      var modelB = new MyModel();
      var fac = new DbContextFactory();
      using var ctx = fac.CreateInMemoryDbContext<TestDbContext>(TestContext);
      _ = ctx.MyModel.Add(modelA);
      _ = ctx.MyModel.Add(modelB);
      _ = await ctx.SaveChangesAsync();
      Assert.AreEqual(2, await ctx.MyModel.CountAsync());
      modelA.UpdatedBy = "Not Me";
      _ = await ctx.SaveChangesAsync();
      Assert.AreEqual("Not Me", modelA.UpdatedBy);
      Assert.IsNull(modelB.UpdatedOnUtc);
    }
  }

  public class MyModel : IAuditable, ICalculateable, IAggregratedByParents
  {
    public int Id { get; set; }
    public int? CalculatedValue { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public IEnumerable<ICalculateable> Parents => new[] { new MyModel() };

    void ICalculateable.CalculateValues()
    {
      CalculatedValue ??= new Random().Next();
    }
  }
  public class TestAuditableContext : AuditableDbContext
  {
    public TestAuditableContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options, httpContextAccessor)
    {
    }
    public DbSet<MyModel> MyModel { get; set; }
  }

}
