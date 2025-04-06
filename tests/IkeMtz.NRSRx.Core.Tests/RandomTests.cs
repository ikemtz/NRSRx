using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class RandomTests : BaseUnigrationTests
  {
    MyIntModel ModelA;
    MyIntModel ModelB;
    TestDbContext DbContext;

    public async Task<DbContext> InitDbContextAsync()
    {
      ModelA = new MyIntModel();
      ModelB = new MyIntModel();

      DbContext = DbContextFactory.CreateInMemoryDbContext<TestDbContext>(TestContext);
      _ = DbContext.MyModel.Add(ModelA);
      _ = DbContext.MyModel.Add(ModelB);
      _ = DbContext.SaveChangesAsync();
      return DbContext;
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task RandomTestWithNoCount()
    {
      await InitDbContextAsync();
      var result = await DbContext.MyModel.RandomAsync();
      Assert.IsNotNull(result);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public async Task RandomTestWitOneCount()
    {
      await InitDbContextAsync();
      var result = await DbContext.MyModel.RandomAsync(1);
    }
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    [ExpectedException(typeof(ArgumentException))]
    public async Task RandomTestWitNull()
    {
      var result = await LinqExtensions.RandomAsync<MyIntModel>(null);
    }
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task RandomTestWitEmptyDbSet()
    {
      DbContext = DbContextFactory.CreateInMemoryDbContext<TestDbContext>(TestContext);
      await DbContext.MyModel.RandomAsync();
    }
  }
}
