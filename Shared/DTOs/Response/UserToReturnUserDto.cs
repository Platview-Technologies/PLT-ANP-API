using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Response
{
    public record UserToReturnUserDto
    {
        public string Email { get; init; }
        public bool IsActive { get; init; }
        public Guid Id { get; init; }
        public string UserId { get; init; }
        public GetUserResponseDto UserModel { get; set; }
        public ICollection<string> Roles { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Role { get; set; }
        public bool MFAEnabled { get; set; }
    }
}
