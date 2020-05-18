using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace IkeMtz.NRSRx.Core.WebApi
{
  public static class ControllerBaseExtensions
  {
    public static string GetBuildNumber(this ControllerBase controller)
    {
      var buildNumber = string.Empty;
      if (controller != null)
      {
        buildNumber = controller.GetType().Assembly.CustomAttributes.FirstOrDefault(t => t.AttributeType == typeof(AssemblyFileVersionAttribute))?.ConstructorArguments[0].Value?.ToString();
      }
      return buildNumber;
    }
  }
}
