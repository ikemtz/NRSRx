using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.NRSRx.Tests;
using IkeMtz.Samples.Events.Redis;
using IkeMtz.Samples.Events.Tests.Integration;
using IkeMtz.Samples.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.Samples.Events.Tests.Unigration
{
  [TestClass]
  public partial class ItemsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task CreateItemsTest()
    {
      var mockPublisher = MockRedisStreamFactory<Item, CreatedEvent>.CreatePublisher();
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationEventsTestStartup>().ConfigureServices(x =>
      {
        x.AddSingleton(mockPublisher.Object);
      }));

      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(Item)}s.json", item);
      _ = resp.EnsureSuccessStatusCode();
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Item>(t => t.Id == item.Id)), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task UpdateItemsTest()
    {
      var mockPublisher = MockRedisStreamFactory<Item, UpdatedEvent>.CreatePublisher();
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationEventsTestStartup>().ConfigureServices(x =>
      {
        x.AddSingleton(mockPublisher.Object);
      }));

      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PutAsJsonAsync($"api/v1/{nameof(Item)}s.json?id={item.Id}", item);
      _ = resp.EnsureSuccessStatusCode();
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Item>(t => t.Id == item.Id)), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task DeleteItemsTest()
    {
      var mockPublisher = MockRedisStreamFactory<Item, DeletedEvent>.CreatePublisher();
      var item = Factories.ItemFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationEventsTestStartup>().ConfigureServices(x =>
      {
        x.AddSingleton(mockPublisher.Object);
      }));

      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"api/v1/{nameof(Item)}s.json?id={item.Id}");
      _ = resp.EnsureSuccessStatusCode();
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Item>(t => t.Id == item.Id)), Times.Once);
    }
  }
}
