using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.Tests
{
    [TestClass]
    public class MockAuditableDbContextFactoryTests
    {
        [TestMethod]
        public async Task MockAuditableContextFactoryTest()
        {
            var modelA = new MyModel();
            var modelB = new MyModel();
            var fac = new DbContextFactory();
            using (var ctx = fac.CreateInMemoryAuditableDbContext<TestAuditableContext>())
            {
                ctx.MyModel.Add(modelA);
                ctx.MyModel.Add(modelB);
                await ctx.SaveChangesAsync();
                Assert.AreEqual(2, await ctx.MyModel.CountAsync());
                modelA.UpdatedBy = "Not Me";
                await ctx.SaveChangesAsync();
                Assert.AreEqual("NRSRx Test User", modelA.UpdatedBy);
                Assert.IsNull(modelB.UpdatedOnUtc);
            }
        }

        [TestMethod]
        public async Task MockContextFactoryTest()
        {
            var modelA = new MyModel();
            var modelB = new MyModel();
            var fac = new DbContextFactory();
            using (var ctx = fac.CreateInMemoryDbContext<TestContext>())
            {
                ctx.MyModel.Add(modelA);
                ctx.MyModel.Add(modelB);
                await ctx.SaveChangesAsync();
                Assert.AreEqual(2, await ctx.MyModel.CountAsync());
                modelA.UpdatedBy = "Not Me";
                await ctx.SaveChangesAsync();
                Assert.AreEqual("Not Me", modelA.UpdatedBy);
                Assert.IsNull(modelB.UpdatedOnUtc);
            }
        }
    }

    public class MyModel : IAuditable
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset CreatedOnUtc { get; set; }
        public DateTimeOffset? UpdatedOnUtc { get; set; }
    }
    public class TestAuditableContext : AuditableDbContext
    {
        public TestAuditableContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options, httpContextAccessor)
        {
        }
        public DbSet<MyModel> MyModel { get; set; }
    }
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<MyModel> MyModel { get; set; }
    }
}
