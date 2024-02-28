

namespace Shared.DTOs.Response
{
    public record DealResponseDto: DealDto
    {
       public Guid Id { get; init; }
    }
}
