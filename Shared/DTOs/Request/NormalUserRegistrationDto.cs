using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Request
{
    public record NormalUserRegistrationDto : UserRegistrationDto
    {
        public string Token { get; set; }
        public Guid TempId { get; set; }
    }
}
