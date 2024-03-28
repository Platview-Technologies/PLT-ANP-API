using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Response
{
    public record GetUserResponseDto
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
    }
}
