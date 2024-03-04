using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Response
{
    public record UserRegTokenDto
    {
        public string regCode { get; set; }
    }
}
