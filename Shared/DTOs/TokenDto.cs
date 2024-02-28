
namespace Shared.DTOs
{
    [Serializable]
    public  record TokenDto(string AccessToken, string RefreshToken);
}
