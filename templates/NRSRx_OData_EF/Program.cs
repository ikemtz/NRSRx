using System.Diagnostics.CodeAnalysis;
using IkeMtz.NRSRx.Core.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NRSRx_OData_EF
{
  [ExcludeFromCodeCoverage]  //This is part of the dotnet aspnet.core project template and typically should not be changed 

  public static class Program
  {
    public static void Main()
    {
      CoreWebStartup.CreateDefaultHostBuilder<Startup>().Build().Run();
    }
  }
}
