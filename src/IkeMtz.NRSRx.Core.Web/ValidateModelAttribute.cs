using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IkeMtz.NRSRx.Core
{
  /// <summary>
  /// An action filter attribute that validates the model state before executing the action.
  /// </summary>
  public sealed class ValidateModelAttribute : ActionFilterAttribute
  {
    /// <summary>
    /// Called before the action method is executed.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      var controller = (ControllerBase)context.Controller;
      var allModelsValid = context.ActionArguments.Values
        .Select(f => controller.TryValidateModel(f)).All(t => t);
      if (!allModelsValid)
      {
        var messages = context.ModelState.SelectMany(t => t.Value.Errors).Select(t => t.ErrorMessage);
        context.Result = new BadRequestObjectResult(messages);
      }
    }
  }
}
