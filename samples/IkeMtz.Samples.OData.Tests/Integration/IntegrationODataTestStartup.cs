using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Data;
using IkeMtz.Samples.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.Samples.OData.Tests.Integration
{
  public class IntegrationODataTestStartup
      : CoreODataIntegrationTestStartup<Startup>
  {
    public IntegrationODataTestStartup(IConfiguration configuration)
        : base(new Startup(configuration))
    {
    }
    public override void SetupAuthentication(AuthenticationBuilder builder)
    {
      builder.SetupTestAuthentication(Configuration, TestContext);
    }
    public override void SetupDatabase(IServiceCollection services, string dbConnectionString)
    {
      _ = services
       .AddDbContextPool<DatabaseContext>(x =>
       {
         _ = x
          .UseSqlServer(dbConnectionString)
          .AddInterceptors(
            new CalculatableTestInterceptor(),
            new AuditableTestInterceptor(MockHttpContextAccessorFactory.CreateAccessor()));
       });
    }
  }
}
