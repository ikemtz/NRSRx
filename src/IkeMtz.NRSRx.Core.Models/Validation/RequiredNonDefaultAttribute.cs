using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.NRSRx.Core.Models.Validation
{
  /// <summary>
  /// Specifies that a data field value is required and must be non-default.
  /// As an example a <see cref="Guid"/> must not be <see cref="Guid.Empty"/>.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
  public sealed class RequiredNonDefaultAttribute : ValidationAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredNonDefaultAttribute"/> class with a default error message.
    /// </summary>
    public RequiredNonDefaultAttribute() : base("The {0} field requires a non-default value.")
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
      if (value is null || Equals(value, Activator.CreateInstance(Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType())))
        return new ValidationResult($"The {validationContext.MemberName} field requires a non-default value.", new[] { validationContext.MemberName });
      return ValidationResult.Success;
    }
  }
}
