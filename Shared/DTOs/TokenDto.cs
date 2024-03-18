
using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    [Serializable]
    public record TokenDto()
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
