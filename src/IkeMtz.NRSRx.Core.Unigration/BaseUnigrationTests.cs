using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class BaseUnigrationTests
  {
    public TestContext TestContext { get; set; }

    public IConfiguration TestServerConfiguration { get; set; }

    public static void GenerateAuthHeader(HttpClient httpClient, string token)
    {
      httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
      var authHeader = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
      httpClient.DefaultRequestHeaders.Authorization = authHeader;
    }

    public string GenerateTestToken(IEnumerable<Claim>? testClaims = null)
    {
      return GenerateTestToken(x =>
     {
       testClaims?.ToList().ForEach(x.Add);
     });
    }

    public string GenerateTestToken(Action<ICollection<Claim>> testClaimsEditor)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("I Love Integration Testing!!!!"));
      var credentials = new SigningCredentials
                    (key, SecurityAlgorithms.HmacSha256Signature);
      var header = new JwtHeader(credentials);

      var claims = new List<Claim>(new[]
      {
          new Claim(JwtRegisteredClaimNames.UniqueName, "Integration Tester"),
          new Claim(JwtRegisteredClaimNames.Email, "IntegrationTester@email.com"),
          new Claim(JwtRegisteredClaimNames.Aud, CoreTestServerExtensions.JwtTokenAud) ,
          new Claim(JwtRegisteredClaimNames.Iss, TestServerConfiguration.GetValue<string>("IdentityProvider") ?? CoreTestServerExtensions.JwtTokenIssuer)
      });
      testClaimsEditor(claims);
      var payload = new JwtPayload(claims);
      var token = new JwtSecurityToken(header, payload);
      var handler = new JwtSecurityTokenHandler();
      // Token to String so you can use it in your client
      var tokenString = handler.WriteToken(token);
      return tokenString;
    }
    public async Task<string> GenerateTokenAsync()
    {
      using var client = new HttpClient();
      var payload = new
      {
        client_id = TestServerConfiguration.GetValue<string>("IntegrationTestClientId"),
        client_secret = TestServerConfiguration.GetValue<string>("IntegrationTestClientSecret"),
        audience = TestServerConfiguration.GetValue<string>("IdentityAudiences").Split(',').First(),
        grant_type = "client_credentials"
      };
      var resp = await client.PostAsJsonAsync(TestServerConfiguration.GetValue<string>("IntegrationTestTokenUrl"), payload).ConfigureAwait(true);
      _ = resp.EnsureSuccessStatusCode();
      var respBody = await resp.Content.ReadAsStringAsync().ConfigureAwait(true);
      TestContext.WriteLine($"Auth0 HttpResponse: {respBody}");
      dynamic o = JsonConvert.DeserializeObject(respBody);
      return o.access_token;
    }

    public void ExecuteOnContext<TDbContext>(IServiceCollection services, Action<TDbContext> callback) where TDbContext : DbContext
    {
      callback = callback ?? throw new ArgumentNullException(nameof(callback));
      // Build the service provider.
      var sp = services.BuildServiceProvider();

      // Create a scope to obtain a reference to the database
      // context (ApplicationDbContext).
      using var scope = sp.CreateScope();
      var scopedServices = scope.ServiceProvider;
      var db = scopedServices.GetRequiredService<TDbContext>();
      // Ensure the database is created.
      try
      {
        _ = db.Database.EnsureCreated();
      }
      catch (Exception ex)
      {
        TestContext.WriteLine($"DB Creation Exception Occured: {ex}");
      }
      if (db is AuditableDbContext)
      {
        var auditableDbContext = db as AuditableDbContext;
        auditableDbContext.CurrentUserProvider = new SystemUserProvider();
      }
      TestContext.WriteLine($"Executing ${nameof(ExecuteOnContext)}<${nameof(TDbContext)}> Logic");
      callback(db);
      _ = db.SaveChanges();
    }

    public async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage httpResponseMessage)
    {
      httpResponseMessage = httpResponseMessage ?? throw new ArgumentNullException(nameof(httpResponseMessage));
      var content = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(true);
      return JsonConvert.DeserializeObject<T>(content, Constants.JsonSerializerSettings);
    }

    public IWebHostBuilder TestHostBuilder<TSiteStartup, TTestStartup>()
        where TTestStartup : class
    {
      Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
      Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
      return new WebHostBuilder()
          .ConfigureAppConfiguration((hostingContext, config) =>
          {
            var appAssembly = typeof(TSiteStartup).Assembly;
            _ = config
             .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
             .AddUserSecrets(appAssembly, optional: true)
             .AddEnvironmentVariables();
          })
           .ConfigureLogging(logging =>
           {
             _ = logging.AddTestContext(TestContext);
           })
           .UseStartup<TTestStartup>()
           .ConfigureServices((webHostBuilderContext, serviceCollection) =>
           {
             _ = serviceCollection
             .AddSingleton(TestContext)
             .AddScoped(x => TestContext);
             TestServerConfiguration = webHostBuilderContext.Configuration;
           });
    }

    public T JsonClone<T>(T source)
    {
      return JsonConvert.DeserializeObject<T>(
        JsonConvert.SerializeObject(source, Constants.JsonSerializerSettings),
          Constants.JsonSerializerSettings);
    }
  }
}
