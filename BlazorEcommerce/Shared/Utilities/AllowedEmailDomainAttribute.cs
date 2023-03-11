using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorEcommerce.Shared.Utilities
{
    public class AllowedEmailDomainAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string email = value?.ToString() ?? "";

            string[] strings = email.ToString().Split('@');

            if (strings[1].ToLower() == "richardgarrison.com" || strings[1].ToLower() == "gmail.com")
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Email domain must be either richardgarrison.com or gmail.com.");
            }
        }
    }
}
