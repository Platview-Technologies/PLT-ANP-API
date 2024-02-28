using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;
using Shared.DTOs.Request;

namespace Service.Contract
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterAdminUser(UserAdminRegistration userAdminRegistration);
        Task<string> GetEmailConfirmationToken(UserModel user);
        Task<IdentityResult> ActivateAccount(AccountActivationByEmailDto accountActivation);
        Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
        Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
    }
}
