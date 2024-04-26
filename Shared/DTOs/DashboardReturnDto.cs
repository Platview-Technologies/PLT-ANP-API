using Shared.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public record DashboardReturnDto
    {
        public IEnumerable<PaginatedDealResponseDto>? Deals { get; set; }
        public int? TotalDeal { get; set; }
        public int? ActiveDeal { get; set; }
        public IEnumerable<NotificationResponseDto>? Notification { get; set; }
        public int? TotalNotification { get; set; }
    }
}
