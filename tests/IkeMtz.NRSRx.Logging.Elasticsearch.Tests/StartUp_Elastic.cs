using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Logging.Elasticsearch.Tests
{
  [DoNotParallelize()]
  public class StartUp_Elastic(IConfiguration configuration) : CoreWebApiStartup(configuration)
  {
    public override void SetupLogging(IServiceCollection? services = null, IApplicationBuilder? app = null) =>
      this.SetupElasticsearch(app);

    public override OpenApiInfo ServiceInfo => new() { Title = "" };

    public override Assembly StartupAssembly => this.GetType().Assembly;
  }
}
