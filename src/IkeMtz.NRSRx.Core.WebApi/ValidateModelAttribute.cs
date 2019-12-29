using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace IkeMtz.NRSRx.Core.WebApi
{
  public sealed class ValidateModelAttribute : ActionFilterAttribute
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods",
      Justification = "ActionExecutingContext is provided by the framework and will never be null")]
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      var controller = (ControllerBase)context.Controller;
      var allModelsValid = context.ActionArguments.Values.Select(f =>
      controller.TryValidateModel(f)).All(t => t);
      if (!allModelsValid)
      {
        var messages = context.ModelState.SelectMany(t => t.Value.Errors).Select(t => t.ErrorMessage);
        context.Result = new BadRequestObjectResult(messages);
      }
    }
  }
}
