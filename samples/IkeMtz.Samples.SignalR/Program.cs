using System;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IkeMtz.Samples.SignalR
{
  public static class Program
  {
    public static void Main() =>
      CoreWebStartup.CreateDefaultHostBuilder<Startup>().Build().Run();
  }
}
