using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IkeMtz.Samples.SignalR.Hubs
{
  [Authorize]
  public class NotificationHub : Hub
  {
    public Task SendMessage(string message) =>
      Clients
        .All
        .SendAsync("OnMessageReceived", $"{Context.User?.Identity?.Name} - {message}");

    public Task SendUserMessage(string receiver, string message) =>
      Clients
        .User(receiver)
        .SendAsync("OnMessageReceived", $"{Context.User?.Identity?.Name} - {message}");
  }
}
