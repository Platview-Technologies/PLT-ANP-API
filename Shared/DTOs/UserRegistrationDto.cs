using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string? confirmPassword { get; init; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; init; }
    }
}
