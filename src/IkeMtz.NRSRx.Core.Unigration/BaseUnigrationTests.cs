using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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

    public void GenerateAuthHeader(HttpClient client, string token)
    {
      var authHeader = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
      client.DefaultRequestHeaders.Authorization = authHeader;
    }

    public string GenerateTestToken(IEnumerable<Claim> testClaims = null)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("I Love Integration Testing!!!!"));
      var credentials = new SigningCredentials
                    (key, SecurityAlgorithms.HmacSha256Signature);
      var header = new JwtHeader(credentials);

      var claims = new List<Claim>(new[]
      {
                new Claim(ClaimTypes.Name, "Integration Tester"),
                new Claim(JwtRegisteredClaimNames.Aud, CoreTestServerExtensions.JwtTokenAud) ,
                new Claim(JwtRegisteredClaimNames.Iss, TestServerConfiguration.GetValue<string>("IdentityProvider") ?? CoreTestServerExtensions.JwtTokenIssuer)
            });
      if (testClaims != null)
      {
        testClaims.ToList().ForEach(claims.Add);
      }
      var payload = new JwtPayload(claims);
      var token = new JwtSecurityToken(header, payload);
      var handler = new JwtSecurityTokenHandler();
      // Token to String so you can use it in your client
      var tokenString = handler.WriteToken(token);
      return tokenString;
    }

    public async Task<string> GenerateTokenAsync()
    {
      var client = new HttpClient();
      var payload = new
      {
        client_id = TestServerConfiguration.GetValue<string>("IntegrationTestClientId"),
        client_secret = TestServerConfiguration.GetValue<string>("IntegrationTestClientSecret"),
        audience = TestServerConfiguration.GetValue<string>("IdentityAudiences").Split(',').First(),
        grant_type = "client_credentials"
      };
      var resp = await client.PostAsJsonAsync(TestServerConfiguration.GetValue<string>("IntegrationTestTokenUrl"), payload);
      resp.EnsureSuccessStatusCode();
      var respBody = await resp.Content.ReadAsStringAsync();
      TestContext.WriteLine($"Auth0 HttpResponse: {respBody}");
      dynamic o = JsonConvert.DeserializeObject(respBody);
      return o.access_token;
    }

    public void ExecuteOnContext<T>(IServiceCollection services, Action<T> callback) where T : DbContext
    {
      // Build the service provider.
      var sp = services.BuildServiceProvider();

      // Create a scope to obtain a reference to the database
      // context (ApplicationDbContext).
      using var scope = sp.CreateScope();
      var scopedServices = scope.ServiceProvider;
      var db = scopedServices.GetRequiredService<T>();
      // Ensure the database is created.
      db.Database.EnsureCreated();
      callback(db);
      db.SaveChanges();
    }

    public async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage resp)
    {
      resp.EnsureSuccessStatusCode();
      if (resp.Content != null)
      {
        var content = await resp.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<T>(content, Constants.JsonSerializerSettings);
        return result;
      }
      return default;
    }

    public IWebHostBuilder TestHostBuilder<SiteStartup, TestStartup>()
        where TestStartup : class
    {
      return new WebHostBuilder()
          .ConfigureAppConfiguration((hostingContext, config) =>
           {
             config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
             var appAssembly = typeof(SiteStartup).Assembly;
             config.AddUserSecrets(appAssembly, optional: true);
           })
           .ConfigureLogging(logging =>
           {
             logging.AddTestContext(TestContext);
           })
           .UseStartup<TestStartup>()
           .ConfigureServices((webHostBuilderContext, serviceCollection) =>
           {
             serviceCollection.AddSingleton(TestContext);
             serviceCollection.AddScoped(x => TestContext);
             TestServerConfiguration = webHostBuilderContext.Configuration;
           });
    }


  }
}
