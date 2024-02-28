using System.ComponentModel.DataAnnotations;


namespace Shared.DTOs.Request
{
    public record UserAdminRegistration: UserRegistrationDto
    {
        [Required(ErrorMessage = "License Code is Required")]
        public string LicenseCode { get; init; }
    }
}
