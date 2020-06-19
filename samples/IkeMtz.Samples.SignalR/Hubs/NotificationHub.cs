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
        .Caller
        .SendAsync("OnMessageRecieved", $"{Context.User.Identity.Name} - {message}");
  }
}
