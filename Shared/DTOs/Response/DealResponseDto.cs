using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Response
{
    public record DealResponseDto: DealDto
    {
       public Guid Id { get; init; }
    }
}
