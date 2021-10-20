using System.Reflection;
using IkeMtz.NRSRx.Core.Web;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Logging.Elasticsearch.Tests
{
  public class StartUp_Elastic : CoreWebApiStartup
  {
    public StartUp_Elastic(IConfiguration configuration) : base(configuration)
    {
    }
    public override void SetupLogging(IServiceCollection services = null, IApplicationBuilder app = null) =>
      this.SetupElasticsearch(app);


    public override string MicroServiceTitle => "";

    public override Assembly StartupAssembly => this.GetType().Assembly;
  }
}
