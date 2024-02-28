using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Request
{
    public record AccountActivationByEmailDto
    {
        [Required(ErrorMessage = "Activation Token Required")]
        public string token { get; set; }
        [Required(ErrorMessage = "User Identification required")]
        public string userId { get; set; }
    }
}
