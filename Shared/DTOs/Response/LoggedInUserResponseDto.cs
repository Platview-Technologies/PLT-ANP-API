using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Response
{
    public record LoggedInUserResponseDto 
    {
        [Required]
        public string UserId { get; init; }
        [Required]
        public bool Authenticated { get; init; }
        [Required]
        public bool ActivatedUser { get; init; }
        
    }
}
