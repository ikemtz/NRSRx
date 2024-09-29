using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.WebApi
{
  /// <summary>
  /// Provides extension methods for the <see cref="ControllerBase"/> class.
  /// </summary>
  public static class ControllerBaseExtensions
  {
    /// <summary>
    /// Gets the build number from the assembly of the specified controller.
    /// </summary>
    /// <param name="controller">The controller instance.</param>
    /// <returns>The build number as a string.</returns>
    public static string GetBuildNumber(this ControllerBase controller)
    {
      var buildNumber = string.Empty;
      if (controller != null)
      {
        buildNumber = controller.GetType().Assembly.CustomAttributes
          .FirstOrDefault(t => t.AttributeType == typeof(AssemblyFileVersionAttribute))?
          .ConstructorArguments[0].Value?.ToString();
      }
      return buildNumber ?? "0.0.0.0";
    }
  }
}
