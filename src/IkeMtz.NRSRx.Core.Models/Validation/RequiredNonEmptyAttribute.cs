using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Models.Validation
{
  /// <summary>
  /// Specifies that a data field value is required and must be non-empty.
  /// As examples a <see cref="string"/> must not be <see cref="string.Empty"/> and a <see cref="ICollection"/> must not be empty.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
  public sealed class RequiredNonEmptyAttribute : ValidationAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredNonEmptyAttribute"/> class with a default error message.
    /// </summary>
    public RequiredNonEmptyAttribute() : base("The {0} field requires a non-default value.")
    {
    }

    /// <summary>
    /// Validates the specified value with respect to the current validation attribute.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="validationContext">The context information about the validation operation.</param>
    /// <returns>An instance of the <see cref="ValidationResult"/> class.</returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value is null || (value is ICollection && !((IEnumerable<object>)value).Any()))
      {
        return new ValidationResult($"The {validationContext.MemberName} field requires a non-empty value.", new[] { validationContext.MemberName });
      }
      return ValidationResult.Success;
    }
  }
}
