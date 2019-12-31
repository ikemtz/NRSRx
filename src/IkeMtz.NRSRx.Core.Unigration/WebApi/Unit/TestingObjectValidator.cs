using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi.Unit
{
  public class TestingObjectValidator : IObjectModelValidator
  {
    public ControllerBase Controller { get; set; }
    public void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
    {
      var validationContext = new ValidationContext(model, null, null);
      var validationResults = new List<ValidationResult>();
      Validator.TryValidateObject(model, validationContext, validationResults, true);
      foreach (var validationResult in validationResults)
      {
        Controller.ModelState.AddModelError(validationResult.MemberNames.FirstOrDefault() ?? string.Empty, validationResult.ErrorMessage);
      }
    }
  }
}
