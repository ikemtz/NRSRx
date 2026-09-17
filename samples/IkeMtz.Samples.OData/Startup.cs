using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IkeMtz.NRSRx.Core.OData;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.OData.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.OData
{
  public class Startup(IConfiguration configuration) : CoreODataStartup(configuration)
  {
    public override int? MaxTop { get; set; } = 500;
    public override string ServiceTitle => $"{nameof(Samples)} OData Microservice";
    public override Assembly StartupAssembly => typeof(Startup).Assembly;
    public override bool IncludeXmlCommentsInSwaggerDocs => true;
    public override string[] AdditionalAssemblyXmlDocumentFiles => new[] {
      typeof(Course).Assembly.Location.Replace(".dll", ".xml", StringComparison.InvariantCultureIgnoreCase)
    };

    public override BaseODataModelProvider ODataModelProvider => new ODataModelProvider();

    [ExcludeFromCodeCoverage]
    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      _ = services
       .AddDbContextPool<DatabaseContext>(x => x.UseSqlServer(dbConnectionString));
    }

    public override void SetupHealthChecks(IServiceCollection services, IHealthChecksBuilder healthChecks)
    {
      _ = healthChecks.AddDbContextCheck<DatabaseContext>();
    }
  }
}
