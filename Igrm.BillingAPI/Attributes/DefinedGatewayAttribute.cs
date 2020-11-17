using Igrm.BillingAPI.Models.Business.Values;
using System;
using System.ComponentModel.DataAnnotations;

namespace Igrm.BillingAPI.Attributes
{
    public class DefinedGatewayAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            Gateway gateway = (Gateway)Convert.ToInt32(value);
            return Enum.IsDefined(gateway) ? ValidationResult.Success : new ValidationResult($"Gateway {gateway} not recognized");
        }
    }
}
