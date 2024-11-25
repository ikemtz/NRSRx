using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class BatchProcessingEntityFrameworkTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task HandleBatchSaveChangesAsyncMultipleDbContext()
    {
      var srcList = new List<CollectionGuidModel> { };
      for (var i = 0; i < 300; i++)
      {
        srcList.Add(new CollectionGuidModel { Id = Guid.NewGuid() });
      }
      var dbContextList = new List<TestAuditableContext>();
      var result = await BatchDataSaver.SaveChangesInBatchAsync<TestAuditableContext, CollectionGuidModel>(() =>
        {
          var dbContext = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableContext>(TestContext);
          dbContextList.Add(dbContext);
          return dbContext;
        }, srcList);
      Assert.AreEqual(300, result);
      Assert.AreEqual(2, dbContextList.Count);
      var _disposedField = typeof(TestAuditableContext).GetField("_disposed", BindingFlags.NonPublic | BindingFlags.Instance);
      dbContextList.ForEach(dbContext =>
        {
          var isDisposed = false;
          try
          {
            dbContext.Database.CanConnect();
          }
          catch (ObjectDisposedException)
          {
            isDisposed = true;
          }
          Assert.IsTrue(isDisposed);
        });
    }


    [TestMethod]
    [TestCategory("Unigration")]
    public async Task HandleBatchSaveChangesAsyncSingleDbContext()
    {
      var context = DbContextFactory.CreateInMemoryAuditableDbContext<TestAuditableDbContext>(TestContext);
      var srcList = new List<CollectionIntModel> { };
      for (var i = 0; i < 300; i++)
      {
        srcList.Add(new CollectionIntModel { Id = i + 1, });
      }
      var result = await context.SaveChangesInBatchAsync(srcList, efAcceptAllChangesOnSuccess: true);
      Assert.AreEqual(300, result);
      Assert.AreEqual(300, await context.CollectionIntModels.CountAsync());
    }
  }
}
