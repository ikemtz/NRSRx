using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace IkeMtz.Samples.SignalR.Hubs
{
  public class NotificationHub : Hub
  {
    public override Task OnConnectedAsync()
    {
      return base.OnConnectedAsync();
    }
    public async Task SendMessage(string user, string message)
    {

      await Clients
          .Caller
          .SendAsync("OnMessageRecieved", $"{user} - {message}").ConfigureAwait(false);
    }
  }
}
