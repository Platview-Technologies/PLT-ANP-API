using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;
using Shared.DTOs.Request;
using Shared.DTOs.Response;

namespace Service.Contract
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterAdminUser(UserAdminRegistrationDto userAdminRegistration);
        Task<IdentityResult> RegisterNormalUser(NormalUserRegistrationDto normalUser);
        Task<string> GetEmailConfirmationToken(UserModel user);
        Task<IdentityResult> ActivateAccount(AccountActivationByEmailDto accountActivation);
        Task<TokenDto> ValidateUser(UserForAuthenticationDto userForAuth);
        //Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
        UserRegTokenDto CreateUserRegCode(string email);
        Task ForgetPassword(NewUserAddDto user);
        Task<IdentityResult> ChangePassword(ForgetPasswordDto forgetPassword);
    }
}
