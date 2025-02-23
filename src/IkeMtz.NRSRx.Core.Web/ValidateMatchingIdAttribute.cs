using System;
using System.Linq;
using IkeMtz.NRSRx.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IkeMtz.NRSRx.Core.Web
{
  /// <summary>
  /// Attribute to validate that the ID in the route matches the ID in the request model.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]

  public class ValidateMatchingIdAttribute : ActionFilterAttribute
  {
    private const string Identifiable = nameof(IIdentifiable);

    /// <summary>
    /// Called before the action method is executed to validate the ID.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      var controller = (ControllerBase)context.Controller;
      context.ActionArguments.TryGetValue("id", out dynamic id);
      if (id == null)
      {
        context.Result = new BadRequestObjectResult("Id is required");
      }
      else
      {
        var requestModel = context.ActionArguments
          .FirstOrDefault(t => t.Value.GetType().GetInterfaces().Any(s => s.Name.StartsWith(Identifiable)));
        if (requestModel.Value == null)
        {
          throw new ArgumentException("An argument that implements IIdentifiable is required");
        }
        else
        {
          var requestType = requestModel.Value.GetType();
          var modelId = requestType.GetProperty("Id").GetValue(requestModel.Value);
          if (!modelId.Equals(id))
          {
            context.Result = new BadRequestObjectResult($"Id querystring parameter does not match the {requestType.Name}.Id value");
          }
        }
      }
    }
  }
}
