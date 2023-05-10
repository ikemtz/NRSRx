using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace IkeMtz.NRSRx.Core
{
  public static class ConfigurationFactory<TProgram>
  {
    private static Assembly configurationAssembly;
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

    public static IConfiguration Create()
    {
      return Configure(new ConfigurationBuilder());
    }

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
