using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IkeMtz.Samples.SignalR.Hubs
{
  [Authorize]
  public class NotificationHub : Hub
  {
    public async Task SendMessage(string message)
    {
      await Clients
          .Caller
          .SendAsync("OnMessageRecieved", $"{this.Context.User.Identity.Name} - {message}").ConfigureAwait(false);
    }
  }
}
