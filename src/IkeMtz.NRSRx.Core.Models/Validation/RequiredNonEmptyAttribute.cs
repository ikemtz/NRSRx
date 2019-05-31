using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IkeMtz.NRSRx.Core.Models.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredNonEmptyAttribute : ValidationAttribute
    {
        public RequiredNonEmptyAttribute() : base("The {0} field requires a non-default value.")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null || (value is ICollection && ((IEnumerable<object>)value).Count() == 0))
            {
                return new ValidationResult($"The {validationContext.MemberName} field requires a non-empty value.", new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }
}
