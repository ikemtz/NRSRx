using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IkeMtz.NRSRx.Core.EntityFramework;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class CollectionHelperTests
  {
    public TestContext TestContext { get; set; }

    [TestMethod]
    [TestCategory("Unit")]
    public void RemoveItemsFromCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModel(), new CollectionModel() };
      context.CollectionModel.AddRange(new[] { srcList.First(), srcList.Last(), new CollectionModel() });
      var destList = context.CollectionModel.ToList();
      context.SyncCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void AddItemsToCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModel(), new CollectionModel() };
      var destList = new List<CollectionModel> { };
      context.SyncCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
    }
  }
}
