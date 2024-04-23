

namespace Shared.DTOs.Response
{
    public record DealResponseDto : DealDto
    {
       public Guid Id { get; init; }
       public NotificationResponseDto LastNotificationSent { get; set; }
       public IEnumerable<NotificationResponseDto> Notifications { get; init; }
    }
}
