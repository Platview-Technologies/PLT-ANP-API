using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities.Validation;

namespace Shared.DTOs.Request
{
    public record NewUserAddDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailValidation(ErrorMessage = "Invalid email address")]
        public string Email { get; init; }
    }
}
