using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public record TwoFactorResponse: IAuthResponse
    {
        public bool Requires2FA { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}
