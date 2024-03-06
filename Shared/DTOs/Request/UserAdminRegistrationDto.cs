using System.ComponentModel.DataAnnotations;
using Utilities.Validation;

namespace Shared.DTOs.Request
{
    public record UserAdminRegistrationDto: UserRegistrationDto
    {
        [Required(ErrorMessage = "License Code is Required")]
        public string LicenseCode { get; init; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailValidation(ErrorMessage = "Invalid email address")]
        public string? Email { get; init; }
    }
}
