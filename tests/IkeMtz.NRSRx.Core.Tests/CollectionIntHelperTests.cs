using System;
using System.Collections.Generic;
using System.Linq;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class CollectionIntHelperTests : BaseUnigrationTests
  {

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullRefOnNullDestinationCollectionTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionIntModel(), new CollectionIntModel() };
      context.SyncIntCollections<CollectionIntModel, CollectionIntModel>(srcList, null, null);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Conversion_NullRefOnNullDestinationCollectionTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionIntModelDto(), new CollectionIntModelDto() };
      context.SyncIntCollections<CollectionIntModelDto, CollectionIntModel>(srcList, null, null);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullRefOnNullContextTest()
    {
      TestAuditableDbContext? context = null;
      var srcList = new[] { new CollectionIntModel(), new CollectionIntModel() };
      var destList = new List<CollectionIntModel>();
      context.SyncIntCollections(srcList, destList, null);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Conversion_NullRefOnNullContextTest()
    {
      TestAuditableDbContext? context = null;
      var srcList = new[] { new CollectionIntModelDto(), new CollectionIntModelDto() };
      var destList = new List<CollectionIntModel>();
      context.SyncIntCollections(srcList, destList, null);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RemoveItemsFromCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionIntModel(), new CollectionIntModel() };
      context.CollectionIntModels.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionIntModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionIntModels.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncIntCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual("Validate Update", destList.First().Value);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Conversion_RemoveItemsFromCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionIntModelDto(), new CollectionIntModelDto() };
      context.CollectionIntModels.AddRange(new[] { srcList.First().ToCollectionModel(), srcList.Last().ToCollectionModel(), new CollectionIntModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionIntModels.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncIntCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual("Validate Update", destList.First().Value);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RemoveItemsFromCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionIntModel(), new CollectionIntModel() };
      context.CollectionIntModels.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionIntModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionIntModels.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncIntCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual("Validate Update", destList.First().Value);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Conversion_RemoveItemsFromCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionIntModelDto(), new CollectionIntModelDto() };
      context.CollectionIntModels.AddRange(new[] { srcList.First().ToCollectionModel(), srcList.Last().ToCollectionModel(), new CollectionIntModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionIntModels.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncIntCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual("Validate Update", destList.First().Value);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void SourceCollectionNullTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionIntModel(), new CollectionIntModel() };
      context.CollectionIntModels.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionIntModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionIntModels.ToList();
      var wasCalled = false;
      context.SyncIntCollections<CollectionIntModel, CollectionIntModel>(null, destList, (src, dest) =>
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
      var srcList = new[] { new CollectionIntModel(), new CollectionIntModel() };
      context.CollectionIntModels.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionIntModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionIntModels.ToList();
      var wasCalled = false;
      context.SyncIntCollections<CollectionIntModelDto, CollectionIntModel>(null, destList, (src, dest) =>
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
      var srcList = new[] { new CollectionIntModel(), new CollectionIntModel() };
      var destList = new List<CollectionIntModel> { };
      context.SyncIntCollections(srcList, destList, (src, dest) =>
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
      var srcList = new[] { new CollectionIntModelDto(), new CollectionIntModelDto() };
      var destList = new List<CollectionIntModel> { };
      context.SyncIntCollections(srcList, destList, (src, dest) =>
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
      var srcList = new[] { new CollectionIntModel(), new CollectionIntModel() };
      var destList = new List<CollectionIntModel> { };
      context.SyncIntCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void Converstion_AddItemsToCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionIntModelDto(), new CollectionIntModelDto() };
      var destList = new List<CollectionIntModel> { };
      context.SyncIntCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
    }
  }
}
