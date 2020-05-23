using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IkeMtz.Samples.WebApi
{
  [ExcludeFromCodeCoverage]
  public static class Program
  {
    public static void Main()
    {
      CreateHostBuilder().Build().Run();
    }

    public static IHostBuilder CreateHostBuilder()
    {
      return Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(webBuilder =>
        {
          _ = webBuilder.UseStartup<Startup>();
        });
    }
  }
}
