namespace Shared.DTOs.Response
{
    public record DealResponseDto : DealDto
    {
       public Guid Id { get; init; }
       public NotificationResponseDto LastNotificationSent { get; set; }
       public IEnumerable<NotificationResponseDto> Notifications { get; init; }
       public bool IsExpired { get; init; }
       public decimal Value { get; init; }
        public IEnumerable<RenewalDealResponseDto> Renewals { get; init; }
    }
}
