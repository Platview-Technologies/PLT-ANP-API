using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Response
{
    public record  ATokenDto
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
