using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NRSRx_WebApi_EF
{
  [ExcludeFromCodeCoverage]
  public static class Program
  {
    public static void Main()
    {
      CoreWebStartup.CreateDefaultHostBuilder<Startup>().Build().Run();
    }
  }
}
