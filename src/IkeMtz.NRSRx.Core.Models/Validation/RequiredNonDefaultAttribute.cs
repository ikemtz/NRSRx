using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.NRSRx.Core.Models.Validation
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]

  public sealed class RequiredNonDefaultAttribute : ValidationAttribute
  {
    public RequiredNonDefaultAttribute() : base("The {0} field requires a non-default value.")
    {
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value is null || Equals(value, Activator.CreateInstance(Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType())))
        return new ValidationResult($"The {validationContext.MemberName} field requires a non-default value.", new[] { validationContext.MemberName });
      return ValidationResult.Success;
    }
  }
}
