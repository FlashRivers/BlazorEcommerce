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
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString().Length == 0)
            {
                return new ValidationResult("Email is required");
            }
            else
            {
                if (!value.ToString().Contains('@'))
                {
                    return new ValidationResult("The Email is invalid");
                }

                string[] strings = value.ToString().Split('@');
                if (strings[1].ToLower() == "richardgarrison.com" || strings[1].ToLower() == "gmail.com")
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Email domain must be either richardgarrison.com or gmail.com");
                }
            }
        }
    }
}
