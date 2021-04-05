using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Tests;
using IkeMtz.Samples.Events.Redis;
using IkeMtz.Samples.Events.Redis.Controllers.V1;
using IkeMtz.Samples.Events.Tests.Integration;
using IkeMtz.Samples.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.Samples.Events.Tests.Unit
{
  [TestClass]
  public partial class ItemsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public async Task CreateItemsTest()
    {
      var mockPublisher = MockRedisStreamPublisherFactory<Item, CreatedEvent>.Create();
      var item = Factories.ItemFactory();
      var result = await new ItemsController().Post(item, mockPublisher.Object)
        as OkObjectResult;
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Item>(t => t.Id == item.Id)), Times.Once);
      Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task UpdateItemsTest()
    {
      var mockPublisher = MockRedisStreamPublisherFactory<Item, UpdatedEvent>.Create();
      var item = Factories.ItemFactory();
      var result = await new ItemsController().Put(item.Id, item, mockPublisher.Object)
        as OkObjectResult;
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Item>(t => t.Id == item.Id)), Times.Once);
      Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public async Task DeleteItemsTest()
    {
      var mockPublisher = MockRedisStreamPublisherFactory<Item, DeletedEvent>.Create();
      var item = Factories.ItemFactory();
      var result = await new ItemsController().Delete(item.Id, mockPublisher.Object)
        as OkObjectResult;
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Item>(t => t.Id == item.Id)), Times.Once);
      Assert.AreEqual(200, result.StatusCode);
    }
  }
}
