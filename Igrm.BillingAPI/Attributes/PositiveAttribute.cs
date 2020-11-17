using System;
using System.ComponentModel.DataAnnotations;

namespace Igrm.BillingAPI.Attributes
{
    public class PositiveAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return Convert.ToDecimal(value) > 0 ? ValidationResult.Success : new ValidationResult("Positive amount expected");
        }
    }
}
