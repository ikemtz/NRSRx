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
  public partial class StudentsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task CreateStudentsTest()
    {
      var mockPublisher = MockRedisStreamFactory<Student, CreatedEvent>.CreatePublisher();
      var item = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationEventsTestStartup>().ConfigureServices(x =>
      {
        _ = x.AddSingleton(mockPublisher.Object);
      }));

      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(Student)}s.json", item);
      var student = await DeserializeResponseAsync<Student>(resp);
      _ = resp.EnsureSuccessStatusCode();
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Student>(t => t.Id == item.Id)), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task UpdateStudentsTest()
    {
      var mockPublisher = MockRedisStreamFactory<Student, UpdatedEvent>.CreatePublisher();
      var item = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationEventsTestStartup>().ConfigureServices(x =>
      {
        _ = x.AddSingleton(mockPublisher.Object);
      }));

      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PutAsJsonAsync($"api/v1/{nameof(Student)}s.json?id={item.Id}", item);
      var student = await DeserializeResponseAsync<Student>(resp);
      _ = resp.EnsureSuccessStatusCode();
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Student>(t => t.Id == item.Id)), Times.Once);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task DeleteStudentsTest()
    {
      var mockPublisher = MockRedisStreamFactory<Student, DeletedEvent>.CreatePublisher();
      var item = Factories.StudentFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationEventsTestStartup>().ConfigureServices(x =>
      {
        _ = x.AddSingleton(mockPublisher.Object);
      }));

      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"api/v1/{nameof(Student)}s.json?id={item.Id}");
      var student = await DeserializeResponseAsync<Student>(resp);
      _ = resp.EnsureSuccessStatusCode();
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Student>(t => t.Id == item.Id)), Times.Once);
    }
  }
}
