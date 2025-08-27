using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.Samples.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.SignalR.Tests
{
  [TestClass]
  public partial class HubTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    [Timeout(4000)]
    public void ValidateSignalRStartup()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
      var startup = new Startup(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
      Assert.IsNull(startup.ServiceTitle);
      Assert.IsNotNull(startup.StartupAssembly);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task NotificationHubTest()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, UnigrationTestStartup>());

      var connection = srv.BuildSignalrConnection("notificationHub", GenerateTestToken());
      var message = "Hello World";
      var returnMessageFired = false;

      _ = connection.On<string>("OnMessageRecieved", msg =>
      {
        Assert.AreEqual($"IntegrationTester@email.com - {message}", msg);
        returnMessageFired = true;
      });

      await connection.StartAsync().ConfigureAwait(false);
      await connection.InvokeAsync("SendMessage", message).ConfigureAwait(true);
      for (int i = 0; i < 10; i++)
      {
        if (returnMessageFired)
        {
          i += 10;
        }
        else
        {
          Thread.Sleep(1000);
        }
      }
      Assert.IsTrue(returnMessageFired);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unigration)]
    public async Task NotificationHub401Test()
    {
      using var srv = new TestServer(TestWebHostBuilder<Startup, Startup>());

      var connection = srv.BuildSignalrConnection("notificationHub", GenerateTestToken());
      await Assert.ThrowsExactlyAsync<HttpRequestException>(async () => await connection.StartAsync().ConfigureAwait(false));
    }
  }
}
