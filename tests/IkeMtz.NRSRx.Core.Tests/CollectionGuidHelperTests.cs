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
  public class CollectionGuidHelperTests : BaseUnigrationTests
  {

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullRefOnNullDestinationCollectionTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModel(), new CollectionGuidModel() };
      context.SyncGuidCollections<CollectionGuidModel, CollectionGuidModel>(srcList, null, null);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Conversion_NullRefOnNullDestinationCollectionTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModelDto(), new CollectionGuidModelDto() };
      context.SyncGuidCollections<CollectionGuidModelDto, CollectionGuidModel>(srcList, null, null);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NullRefOnNullContextTest()
    {
      TestAuditableDbContext? context = null;
      var srcList = new[] { new CollectionGuidModel(), new CollectionGuidModel() };
      var destList = new List<CollectionGuidModel>();
      context.SyncGuidCollections(srcList, destList, null);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Conversion_NullRefOnNullContextTest()
    {
      TestAuditableDbContext? context = null;
      var srcList = new[] { new CollectionGuidModelDto(), new CollectionGuidModelDto() };
      var destList = new List<CollectionGuidModel>();
      context.SyncGuidCollections(srcList, destList, null);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void RemoveItemsFromCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModel(), new CollectionGuidModel() };
      context.CollectionGuidModels.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionGuidModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionGuidModels.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncGuidCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual("Validate Update", destList.First().Value);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Conversion_RemoveItemsFromCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModelDto(), new CollectionGuidModelDto() };
      context.CollectionGuidModels.AddRange(new[] { srcList.First().ToCollectionModel(), srcList.Last().ToCollectionModel(), new CollectionGuidModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionGuidModels.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncGuidCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual("Validate Update", destList.First().Value);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void RemoveItemsFromCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModel(), new CollectionGuidModel() };
      context.CollectionGuidModels.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionGuidModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionGuidModels.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncGuidCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual("Validate Update", destList.First().Value);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Conversion_RemoveItemsFromCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModelDto(), new CollectionGuidModelDto() };
      context.CollectionGuidModels.AddRange(new[] { srcList.First().ToCollectionModel(), srcList.Last().ToCollectionModel(), new CollectionGuidModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionGuidModels.ToList();
      srcList.First().Value = "Validate Update";
      context.SyncGuidCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
      Assert.AreEqual("Validate Update", destList.First().Value);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void SourceCollectionNullTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModel(), new CollectionGuidModel() };
      context.CollectionGuidModels.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionGuidModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionGuidModels.ToList();
      var wasCalled = false;
      context.SyncGuidCollections<CollectionGuidModel, CollectionGuidModel>(null, destList, (src, dest) =>
      {
        wasCalled = true;
      });
      Assert.AreEqual(0, destList.Count);
      Assert.IsFalse(wasCalled);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Conversion_SourceCollectionNullTest()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModel(), new CollectionGuidModel() };
      context.CollectionGuidModels.AddRange(new[] { srcList.First().Clone(), srcList.Last().Clone(), new CollectionGuidModel() });
      _ = context.SaveChanges();
      var destList = context.CollectionGuidModels.ToList();
      var wasCalled = false;
      context.SyncGuidCollections<CollectionGuidModelDto, CollectionGuidModel>(null, destList, (src, dest) =>
      {
        wasCalled = true;
      });
      Assert.AreEqual(0, destList.Count);
      Assert.IsFalse(wasCalled);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void AddItemsToCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModel(), new CollectionGuidModel() };
      var destList = new List<CollectionGuidModel> { };
      context.SyncGuidCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Converstion_AddItemsToCollection()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModelDto(), new CollectionGuidModelDto() };
      var destList = new List<CollectionGuidModel> { };
      context.SyncGuidCollections(srcList, destList, (src, dest) =>
     {
       dest.Value = src.Value;
     });
      Assert.AreEqual(2, destList.Count);
    }


    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void AddItemsToCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModel(), new CollectionGuidModel() };
      var destList = new List<CollectionGuidModel> { };
      context.SyncGuidCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public void Converstion_AddItemsToCollection_SimpleMapper()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new[] { new CollectionGuidModelDto(), new CollectionGuidModelDto() };
      var destList = new List<CollectionGuidModel> { };
      context.SyncGuidCollections(srcList, destList);
      Assert.AreEqual(2, destList.Count);
    }
  }
}
