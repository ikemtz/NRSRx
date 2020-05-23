using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NRSRx_WebApi_EF
{
  [ExcludeFromCodeCoverage]
  public static class Program
  {
    public static void Main()
    {
      CreateHostBuilder(Array.Empty<string>()).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          _ = webBuilder.UseStartup<Startup>();
        });
    }
  }
}
