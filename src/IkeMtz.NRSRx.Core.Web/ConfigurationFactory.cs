using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.Core
{
  /// <summary>
  /// Provides methods to create and configure application configuration.
  /// </summary>
  /// <typeparam name="TProgram">The type of the program.</typeparam>
  public static class ConfigurationFactory<TProgram>
  {
    private static Assembly configurationAssembly;

    /// <summary>
    /// Gets the assembly that defines the TProgram type.
    /// </summary>
    public static Assembly ConfigurationAssembly
    {
      get
      {
        if (configurationAssembly == null)
        {
          configurationAssembly = typeof(TProgram).Assembly;
        }
        return configurationAssembly;
      }
    }

    /// <summary>
    /// Creates a new configuration instance.
    /// </summary>
    /// <returns>The created configuration.</returns>
    public static IConfiguration Create()
    {
      return Configure(new ConfigurationBuilder());
    }

    /// <summary>
    /// Configures the specified configuration builder.
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <returns>The configured configuration.</returns>
    public static IConfiguration Configure(IConfigurationBuilder builder)
    {
      return builder
         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
         .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: false)
         .AddUserSecrets(ConfigurationAssembly)
         .AddEnvironmentVariables()
         .Build();
    }
  }
}
