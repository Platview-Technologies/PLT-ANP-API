﻿using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Shared.DTOs;
using Shared.DTOs.Request;

namespace Service.Contract
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterAdminUser(UserAdminRegistrationDto userAdminRegistration);
        Task<string> GetEmailConfirmationToken(UserModel user);
        Task<IdentityResult> ActivateAccount(AccountActivationByEmailDto accountActivation);
        Task<TokenDto> ValidateUser(UserForAuthenticationDto userForAuth);
        //Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
    }
}
