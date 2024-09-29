using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace IkeMtz.NRSRx.Core.Unigration.WebApi.Unit
{
  /// <summary>
  /// Provides an object validator for unit testing purposes.
  /// </summary>
  public class TestingObjectValidator : IObjectModelValidator
  {
    /// <summary>
    /// Gets or sets the controller associated with this validator.
    /// </summary>
    public ControllerBase Controller { get; set; }

    /// <summary>
    /// Validates the specified model and updates the model state of the associated controller.
    /// </summary>
    /// <param name="actionContext">The context of the action.</param>
    /// <param name="validationState">The validation state dictionary.</param>
    /// <param name="prefix">The prefix to use when looking up values in the model state dictionary.</param>
    /// <param name="model">The model to validate.</param>
    public void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
    {
      var validationContext = new ValidationContext(model, null, null);
      var validationResults = new List<ValidationResult>();
      _ = Validator.TryValidateObject(model, validationContext, validationResults, true);
      foreach (var validationResult in validationResults)
      {
        Controller.ModelState.AddModelError(validationResult.MemberNames.FirstOrDefault() ?? string.Empty, validationResult.ErrorMessage);
      }
    }
  }
}
