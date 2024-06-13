using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.Response
{
    public record DealRenewalResponseDto: DealDto
    {
        public Guid Id { get; init; }
        public bool IsExpired { get; init; }
        public decimal Value { get; init; }
    }
}
