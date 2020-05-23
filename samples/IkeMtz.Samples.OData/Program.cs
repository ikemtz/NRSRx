using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace IkeMtz.Samples.OData
{
  public static class Program
  {
    public static void Main()
    {
      CreateHostBuilder().Build().Run();
    }

    public static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
              _ = webBuilder.UseStartup<Startup>();
            });
  }
}
