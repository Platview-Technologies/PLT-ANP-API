

namespace Shared.DTOs.Response
{
    public record PaginatedDealResponseDto : DealDto
    {
       public Guid Id { get; init; }
       public NotificationResponseDto LastNotificationSent { get; set; }
    }
}
