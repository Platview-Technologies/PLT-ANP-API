using System.ComponentModel.DataAnnotations;


namespace Shared.DTOs
{
    public abstract record UserRegistrationDto
    {
        [Required(ErrorMessage = "FirstName is Required!")]
        public string? FirstName { get; init; }
        [Required(ErrorMessage = "LastName is Required!")]
        public string? LastName { get; init; }
        public string? Username { get; init; } 
        [Required(ErrorMessage = "Password is Required!")]
        [DataType(DataType.Password)]
        public string? Password { get; init; }
        [Required(ErrorMessage = "Confirm Password is Required!")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match!")]
        public string? ConfirmPassword { get; init; }
        
    }
}
