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
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Unigration.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Serilog;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Base class for integration tests, providing common functionality for setting up and running tests.
  /// </summary>
  [DoNotParallelize]
  public class BaseUnigrationTests
  {
    public static readonly string CONTROLLER_NAME_SUFFIX = "Controller";
    public static readonly int CONTROLLER_NAME_SUFFIX_CHAR_COUNT = 10;
    /// <summary>
    /// Gets or sets the test context.
    /// </summary>
    public TestContext TestContext { get; set; }

    /// <summary>
    /// Gets or sets the test server configuration.
    /// </summary>
    public IConfiguration TestServerConfiguration { get; set; }

    /// <summary>
    /// Generates an authentication header for the specified HTTP client using the provided token.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="token">The token.</param>
    public static void GenerateAuthHeader(HttpClient httpClient, string token)
    {
      httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
      var authHeader = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
      httpClient.DefaultRequestHeaders.Authorization = authHeader;
    }

    /// <summary>
    /// Generates a test JWT token with the specified claims.
    /// </summary>
    /// <param name="testClaims">The test claims.</param>
    /// <returns>The generated JWT token.</returns>
    public string GenerateTestToken(IEnumerable<Claim>? testClaims = null)
    {
      return GenerateTestToken(x =>
     {
       testClaims?.ToList().ForEach(x.Add);
     });
    }

    /// <summary>
    /// Generates a test JWT token using the specified claims editor action.
    /// </summary>
    /// <param name="testClaimsEditor">The claims editor action.</param>
    /// <returns>The generated JWT token.</returns>
    public string GenerateTestToken(Action<ICollection<Claim>> testClaimsEditor)
    {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("I Love NRSRx Integration Testing!!!!"));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
      var header = new JwtHeader(credentials);

      var claims = new List<Claim>
          {
              new(JwtRegisteredClaimNames.UniqueName, "Integration Tester"),
              new(JwtRegisteredClaimNames.Email, "IntegrationTester@email.com"),
              new(JwtRegisteredClaimNames.Sub, "IntegrationTester@subject.com"),
              new(JwtRegisteredClaimNames.Aud, CoreTestServerExtensions.JwtTokenAud),
              new(JwtRegisteredClaimNames.Iss, TestServerConfiguration.GetValue<string>("IdentityProvider") ?? CoreTestServerExtensions.JwtTokenIssuer)
          };
      testClaimsEditor(claims);
      var payload = new JwtPayload(claims);
      var token = new JwtSecurityToken(header, payload);
      var handler = new JwtSecurityTokenHandler();
      var tokenString = handler.WriteToken(token);
      return tokenString;
    }

    /// <summary>
    /// Generates a token asynchronously by making a request to the identity server.
    /// </summary>
    /// <returns>The generated token.</returns>
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
      var respBody = await resp.Content.ReadAsStringAsync(TestContext.CancellationToken).ConfigureAwait(true);
      TestContext.WriteLine($"Identity Server HttpResponse: {respBody}");
      dynamic o = JsonConvert.DeserializeObject(respBody);
      return o.access_token;
    }

    /// <summary>
    /// Executes the specified callback on the provided DbContext.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="callback">The callback action.</param>
    public void ExecuteOnContext<TDbContext>(IServiceCollection services, Action<TDbContext> callback) where TDbContext : DbContext
    {
      callback = callback ?? throw new ArgumentNullException(nameof(callback));
      var sp = services.BuildServiceProvider();
      using var scope = sp.CreateScope();
      var scopedServices = scope.ServiceProvider;
      var db = scopedServices.GetRequiredService<TDbContext>();
      var logger = scopedServices.GetRequiredService<ILogger<TDbContext>>();
      try
      {
        _ = db.Database.EnsureCreated();
      }
      catch (Exception ex)
      {
        TestContext.WriteLine($"DB Creation Exception Occurred: {ex}");
      }
      TestContext.WriteLine($"Executing {nameof(ExecuteOnContext)}<{nameof(TDbContext)}> Logic");
      callback(db);
      if (db is AuditableDbContext auditableDbContext)
      {
        auditableDbContext.CurrentUserProvider = new SystemUserProvider();
        _ = auditableDbContext.SaveChanges(logger);
      }
      else
      {
        _ = db.SaveChanges();
      }
    }

    /// <summary>
    /// Deserializes the response content to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="httpResponseMessage">The HTTP response message.</param>
    /// <returns>The deserialized object.</returns>
    public async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage httpResponseMessage)
    {
      httpResponseMessage = httpResponseMessage ?? throw new ArgumentNullException(nameof(httpResponseMessage));
      var content = await httpResponseMessage.Content.ReadAsStringAsync(TestContext.CancellationToken).ConfigureAwait(true);
      return JsonConvert.DeserializeObject<T>(content, Constants.JsonSerializerSettings);
    }

    /// <summary>
    /// Creates a test web host builder with the specified startup classes.
    /// </summary>
    /// <typeparam name="TSiteStartup">The type of the site startup class.</typeparam>
    /// <typeparam name="TTestStartup">The type of the test startup class.</typeparam>
    /// <returns>The web host builder.</returns>
    public IWebHostBuilder TestWebHostBuilder<TSiteStartup, TTestStartup>()
        where TTestStartup : class
    {
      Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
      Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
      return new WebHostBuilder()
          .ConfigureAppConfiguration((hostingContext, config) =>
          {
            _ = ConfigurationFactory<TSiteStartup>.Configure(config);
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
                 .AddScoped(x => TestContext)
                 .AddSerilog();
             TestServerConfiguration = webHostBuilderContext.Configuration;
           });
    }

    /// <summary>
    /// Creates a deep copy of the specified object using JSON serialization.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="source">The source object.</param>
    /// <returns>The deep copy of the object.</returns>
    public T JsonClone<T>(T source)
    {
      return JsonConvert.DeserializeObject<T>(
        JsonConvert.SerializeObject(source, Constants.JsonSerializerSettings),
          Constants.JsonSerializerSettings);
    }

    /// <summary>
    /// Gets the controller route name for the specified controller type.
    /// This strips the trailing "Controller" suffix from the type name when present.
    /// </summary>
    /// <typeparam name="T_CONTROLLER">The controller type.</typeparam>
    /// <returns>The controller route name (type name without the "Controller" suffix when applicable).</returns>
    public string GetControllerRoute<T_CONTROLLER>()
      where T_CONTROLLER : ControllerBase
    {
      var type = typeof(T_CONTROLLER);
      return GetControllerRouteName(type.Name);
    }


    /// <summary>
    /// Builds the full route for the specified controller type including API version and
    /// whether the controller is an OData controller or a normal Web API controller.
    /// </summary>
    /// <typeparam name="T_CONTROLLER">The controller type.</typeparam>
    /// <returns>
    /// A route string in the form of "odata/v{majorVersion}/{controllerName}" for OData controllers
    /// or "api/v{majorVersion}/{controllerName}.json" for regular controllers.
    /// </returns>
    public string GetFullRoute<T_CONTROLLER>()
      where T_CONTROLLER : ControllerBase
    {
      var type = typeof(T_CONTROLLER);
      var controllerName = GetControllerRouteName(type.Name);
      var odataControllerType = typeof(ODataController);
      var majorVersionValue = type.GetCustomAttribute<ApiVersionAttribute>()?.Versions.FirstOrDefault()?.MajorVersion;

      if (type.BaseType == odataControllerType || type.BaseType.BaseType == odataControllerType)
      {
        return $"odata/v{majorVersionValue}/{controllerName}";
      }
      else
      {
        return $"api/v{majorVersionValue}/{controllerName}.json";
      }
    }

    /// <summary>
    /// Returns the route name for a controller type name by removing the standard
    /// "Controller" suffix if it exists. If the name does not end with the suffix,
    /// the original name is returned and a warning is written to the test context.
    /// </summary>
    /// <param name="controllerName">The controller type name.</param>
    /// <returns>The controller route name without the "Controller" suffix when applicable.</returns>
    public string GetControllerRouteName(string controllerName)
    {
      if (controllerName.EndsWith(CONTROLLER_NAME_SUFFIX, StringComparison.CurrentCultureIgnoreCase))
      {
        return controllerName[..^CONTROLLER_NAME_SUFFIX_CHAR_COUNT];
      }
      TestContext.WriteLine("WARNING: {controllerName} does not meet the expected {{ENTITY_NAME}}Controller format.", controllerName);
      return controllerName;
    }

    public string GetODataEnvelopeName<T_ENTITY>()
      where T_ENTITY: IIdentifiable<Guid>, IIdentifiable
    {
      return GetODataEnvelopeName<T_ENTITY, Guid>();
    }
    public string GetODataEnvelopeName<T_ENTITY, T_IDENTITY_TYPE>()
      where T_IDENTITY_TYPE: IComparable
      where T_ENTITY : IIdentifiable<T_IDENTITY_TYPE>
 
    {
      return $"ODataEnvelopeOf{typeof(T_ENTITY).Name}And{typeof(T_IDENTITY_TYPE).Name}";
    }
  }
}
