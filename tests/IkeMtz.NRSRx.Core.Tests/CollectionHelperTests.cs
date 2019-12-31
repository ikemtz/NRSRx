using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class CollectionHelperTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void RemoveItemsFromCollection()
    {
      var srcList = new List<CollectionModel> { new CollectionModel(), new CollectionModel() };
      var destList = new List<CollectionModel> { srcList.First(), srcList.Last(), new CollectionModel() };
      CollectionHelper<CollectionModel>.SyncCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void AddItemsToCollection()
    {
      var srcList = new List<CollectionModel> { new CollectionModel(), new CollectionModel() };
      var destList = new List<CollectionModel> { };
      CollectionHelper<CollectionModel>.SyncCollections(srcList, destList, (src, dest) =>
      {
        dest.Value = src.Value;
      });
      Assert.AreEqual(2, destList.Count);
    }
  }
}
