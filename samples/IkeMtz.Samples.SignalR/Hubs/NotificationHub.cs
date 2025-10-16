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
        .SendAsync("OnMessageRecieved", $"{Context.User?.Identity?.Name} - {message}");

    public Task SendUserMessage(string receiver, string message) =>
      Clients
        .User(receiver)
        .SendAsync("OnMessageRecieved", $"{Context.User?.Identity?.Name} - {message}");
  }
}
