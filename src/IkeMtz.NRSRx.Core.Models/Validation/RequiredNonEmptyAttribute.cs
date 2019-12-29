using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Models.Validation
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
  public sealed class RequiredNonEmptyAttribute : ValidationAttribute
  {
    public RequiredNonEmptyAttribute() : base("The {0} field requires a non-default value.")
    {
    }

    [SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "ValidationContext is provided by the framework and would never bel null")]
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
