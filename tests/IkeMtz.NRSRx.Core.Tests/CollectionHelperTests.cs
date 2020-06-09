using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class CollectionHelperTests
  {
    public TestContext TestContext { get; set; }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullRefOnNullDestinationCollectionTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModel(), new CollectionModel() };
      context.SyncCollections(srcList, null, null);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Conversion_NullRefOnNullDestinationCollectionTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModelDto(), new CollectionModelDto() };
      context.SyncCollections<CollectionModelDto, CollectionModel>(srcList, null, null);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullRefOnNullContextTest()
    {
      TestAuditableDbContext context = null;
      var srcList = new[] { new CollectionModel(), new CollectionModel() };
      var destList = new List<CollectionModel>();
      context.SyncCollections(srcList, destList, null);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Conversion_NullRefOnNullContextTest()
    {
      TestAuditableDbContext context = null;
      var srcList = new[] { new CollectionModelDto(), new CollectionModelDto() };
      var destList = new List<CollectionModel>();
      context.SyncCollections(srcList, destList, null);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RemoveItemsFromCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModel(), new CollectionModel() };
      context.CollectionModel.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionModel.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual(destList.First().Value, "Validate Update");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Conversion_RemoveItemsFromCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModelDto(), new CollectionModelDto() };
      context.CollectionModel.AddRange(new[] { srcList.First().ToCollectionModel(), srcList.Last().ToCollectionModel(), new CollectionModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionModel.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual(destList.First().Value, "Validate Update");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RemoveItemsFromCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModel(), new CollectionModel() };
      context.CollectionModel.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionModel.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual(destList.First().Value, "Validate Update");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Conversion_RemoveItemsFromCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModelDto(), new CollectionModelDto() };
      context.CollectionModel.AddRange(new[] { srcList.First().ToCollectionModel(), srcList.Last().ToCollectionModel(), new CollectionModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionModel.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual(destList.First().Value, "Validate Update");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void SourceCollectionNullTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModel(), new CollectionModel() };
      context.CollectionModel.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionModel.ToList();
      var wasCalled = false;
      context.SyncCollections(null, destList, (src, dest) =>
      {
        wasCalled = true;
      });
      Assert.AreEqual(3, destList.Count);
      Assert.IsFalse(wasCalled);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Conversion_SourceCollectionNullTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModel(), new CollectionModel() };
      context.CollectionModel.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionModel.ToList();
      var wasCalled = false;
      context.SyncCollections<CollectionModelDto, CollectionModel>(null, destList, (src, dest) =>
      {
        wasCalled = true;
      });
      Assert.AreEqual(3, destList.Count);
      Assert.IsFalse(wasCalled);
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

    [TestMethod]
    [TestCategory("Unit")]
    public void Converstion_AddItemsToCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModelDto(), new CollectionModelDto() };
      var destList = new List<CollectionModel> { };
      context.SyncCollections(srcList, destList, (src, dest) =>
     {
       dest.Value = src.Value;
     });
      Assert.AreEqual(2, destList.Count);
    }


    [TestMethod]
    [TestCategory("Unit")]
    public void AddItemsToCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModel(), new CollectionModel() };
      var destList = new List<CollectionModel> { };
      context.SyncCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Converstion_AddItemsToCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionModelDto(), new CollectionModelDto() };
      var destList = new List<CollectionModel> { };
      context.SyncCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
    }
  }
}
