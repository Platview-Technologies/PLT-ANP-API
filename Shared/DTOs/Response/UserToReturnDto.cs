using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Response
{
    public record UserToReturnDto
    {
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public Guid id { get; set; }

    }
}
