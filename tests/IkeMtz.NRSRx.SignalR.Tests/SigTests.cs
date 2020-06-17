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
  public class SigTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    [Timeout(4000)]
    public async Task NotificationHubTest()
    {
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());

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
      while (!returnMessageFired)
      {
        Thread.Sleep(500);
      }
      Assert.IsTrue(returnMessageFired);
    }
  }
}
