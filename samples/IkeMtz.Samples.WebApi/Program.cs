using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IkeMtz.Samples.WebApi
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
