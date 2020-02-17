using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static class CoreTestServerExtensions
  {
    public const string JwtTokenIssuer = "UnigrationTestOAuthServer";
    public const string JwtTokenAud = "@IkeMtz";
    public static void SetupTestAuthentication(this AuthenticationBuilder builder, IConfiguration Configuration, TestContext testContext)
    {
      builder
      .AddJwtBearer(options =>
      {
        options.Events = new JwtBearerEvents()
        {
          OnMessageReceived = x =>
                {
                  var bearer = x.Request.Headers["Authorization"].ToString().Split(" ").Last();
                  if (!string.IsNullOrWhiteSpace(bearer))
                  {
                    var token = new JwtSecurityToken(bearer);
                    var identity = new ClaimsIdentity(token.Claims, "IntegrationTest");
                    x.Principal = new ClaimsPrincipal(new[] { identity });
                    x.Success();
                  }
                  else
                  {
                    testContext?.WriteLine("*** UnauthorizedAccessException ***");
                    testContext?.WriteLine(" No Authorization header provided. ");
                    x.Fail(new UnauthorizedAccessException("No Authorization header provided."));
                  }
                  return Task.CompletedTask;
                },
          OnAuthenticationFailed = x =>
           {
             testContext?.WriteLine("*** Authentication Failed ***");
             testContext?.WriteLine($"Exception: {x.Exception?.Message}");
             testContext?.WriteLine($"Failure: {x.Result?.Failure?.Message}");
             x.Request?.Headers?.ToList().ForEach(header => testContext?.WriteLine($"Header - {header.Key}: {header.Value}"));
             return Task.CompletedTask;
           }

        };
        options.Authority = Configuration.GetValue<string>("IdentityProvider") ?? JwtTokenIssuer;
        options.Audience = JwtTokenAud;
        options.RequireHttpsMetadata = false;
        options.IncludeErrorDetails = true;
      });
    }

    public static void SetupTestDbContext<T>(this IServiceCollection services) where T : DbContext
    {
      // Build the service provider.
      var serviceProvider = services
          .AddEntityFrameworkInMemoryDatabase()
          .AddScoped<ILoggerFactory>(provider => new LoggerFactory(new[] {
            new TestContextLoggerProvider(provider.GetService<TestContext>()) }))
          .BuildServiceProvider();
      var testContext = serviceProvider.GetService<TestContext>();
      services.AddDbContext<T>(options =>
      {
        options.UseInMemoryDatabase($"InMemoryDbForTesting-{testContext.TestName}");
        options.UseInternalServiceProvider(serviceProvider);
        options.EnableSensitiveDataLogging(true);
        options.EnableDetailedErrors(true);
      });
    }
  }
}
