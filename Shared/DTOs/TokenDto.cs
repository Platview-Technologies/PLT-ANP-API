
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    [Serializable]
    public record TokenDto: IAuthResponse
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public Guid DeviceId { get; set; }

    }
}
