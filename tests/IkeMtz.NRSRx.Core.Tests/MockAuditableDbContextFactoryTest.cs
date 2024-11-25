using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Data;
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
      var modelA = new MyIntModel();
      var modelB = new MyIntModel();
      using var ctx = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableContext>(TestContext);
      _ = ctx.MyModels.Add(modelA);
      _ = ctx.MyModels.Add(modelB);
      _ = await ctx.SaveChangesAsync();
      Assert.AreEqual(2, await ctx.MyModels.CountAsync());
      modelA.UpdatedBy = "Not Me";
      _ = await ctx.SaveChangesAsync();
      Assert.AreEqual(SystemUserProvider.SystemUserId, modelA.UpdatedBy);
      Assert.IsNull(modelB.UpdatedOnUtc);
      Assert.IsNull(modelB.UpdateCount);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task MockAuditableContextAddAsyncTest()
    {
      var modelA = new MyIntModel();
      var modelB = new MyIntModel();
      using var ctx = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableContext>(TestContext);
      var dbContextObjA = await ctx.AddAsync(modelA);
      var dbContextObjB = await ctx.AddAsync<MyIntModel>(modelB);
      Assert.AreEqual(1, dbContextObjA.Entity.Id);
      Assert.AreEqual(2, dbContextObjB.Entity.Id);
      Assert.IsNotNull(dbContextObjA.Entity.CalculatedValue);
      Assert.IsNotNull(dbContextObjB.Entity.CalculatedValue);
    }
    [TestMethod]
    [TestCategory("Unit")]
    public async Task MockDbContextFactoryTest()
    {
      var modelA = new MyIntModel();
      var modelB = new MyIntModel();
      using var ctx = DbContextFactory.CreateInMemoryDbContext<TestDbContext>(TestContext);
      _ = ctx.MyModel.Add(modelA);
      _ = ctx.MyModel.Add(modelB);
      _ = await ctx.SaveChangesAsync();
      Assert.AreEqual(2, await ctx.MyModel.CountAsync());
      modelA.UpdatedBy = "Not Me";
      _ = await ctx.SaveChangesAsync();
      Assert.AreEqual(SystemUserProvider.SystemUserId, modelA.UpdatedBy);
      Assert.IsNull(modelB.UpdatedOnUtc);
    }
  }

  public class MyIntModel : IIdentifiable<int>, IAuditable, ICalculateable
  {
    public int Id { get; set; }
    public int? CalculatedValue { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public IEnumerable<ICalculateable> Parents => [new MyIntModel()];

    public int? UpdateCount { get; set; }

    void ICalculateable.CalculateValues()
    {
      CalculatedValue ??= new Random().Next();
    }
  }

  public class MyGuidModel : IIdentifiable, IAuditable, ICalculateable
  {
    public Guid Id { get; set; }
    public int? CalculatedValue { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }
    public int? UpdateCount { get; set; }
    public IEnumerable<ICalculateable> Parents => [new MyIntModel()];

    void ICalculateable.CalculateValues()
    {
      CalculatedValue ??= new Random().Next();
    }
  }
  public class TestAuditableContext(DbContextOptions options, ICurrentUserProvider currentUserProvider) : AuditableDbContext(options, currentUserProvider)
  {
    public DbSet<MyIntModel> MyModels { get; set; }
    public DbSet<CollectionIntModel> CollectionIntModels { get; set; }
    public DbSet<CollectionGuidModel> CollectionGuidModels { get; set; }
  }

}
