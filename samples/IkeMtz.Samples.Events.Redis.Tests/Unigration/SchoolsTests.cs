using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Core.Unigration.Http;
using IkeMtz.NRSRx.Events;
using IkeMtz.Samples.Events.Redis;
using IkeMtz.Samples.Events.Tests.Integration;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Tests;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.Samples.Events.Tests.Unigration
{
  [TestClass]
  public partial class SchoolsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task CreateSchoolsTest()
    {
      var mockPublisher = MockRedisStreamFactory<School, CreatedEvent>.CreatePublisher();
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationEventsTestStartup>().ConfigureServices(x =>
      {
        _ = x.AddSingleton(mockPublisher.Object);
      }));

      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(School)}s.json", item);
      var school = await DeserializeResponseAsync<School>(resp);
      _ = resp.EnsureSuccessStatusCode();
      mockPublisher.Verify(t => t.PublishAsync(It.Is<School>(t => t.Id == item.Id)), Times.Once);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task UpdateSchoolsTest()
    {
      var mockPublisher = MockRedisStreamFactory<School, UpdatedEvent>.CreatePublisher();
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationEventsTestStartup>().ConfigureServices(x =>
      {
        _ = x.AddSingleton(mockPublisher.Object);
      }));

      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PutAsJsonAsync($"api/v1/{nameof(School)}s.json?id={item.Id}", item);
      var school = await DeserializeResponseAsync<School>(resp);
      _ = resp.EnsureSuccessStatusCode();
      mockPublisher.Verify(t => t.PublishAsync(It.Is<School>(t => t.Id == item.Id)), Times.Once);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task DeleteSchoolsTest()
    {
      var mockPublisher = MockRedisStreamFactory<School, DeletedEvent>.CreatePublisher();
      var item = Factories.SchoolFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationEventsTestStartup>().ConfigureServices(x =>
      {
        _ = x.AddSingleton(mockPublisher.Object);
      }));

      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"api/v1/{nameof(School)}s.json?id={item.Id}");
      var school = await DeserializeResponseAsync<School>(resp);
      _ = resp.EnsureSuccessStatusCode();
      mockPublisher.Verify(t => t.PublishAsync(It.Is<School>(t => t.Id == item.Id)), Times.Once);
    }
  }
}
