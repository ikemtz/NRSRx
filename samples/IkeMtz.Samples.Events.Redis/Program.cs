using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace IkeMtz.Samples.Events.Redis
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
