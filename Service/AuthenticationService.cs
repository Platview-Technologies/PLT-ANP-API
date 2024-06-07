using AutoMapper;
using Contract;
using Contracts;
using Entities.ConfigurationModels;
using Entities.Exceptions;
using Entities.Models;
using Entities.SystemModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit.Cryptography;
using OtpNet;
using Service.Contract;
using Shared.DTOs;
using Shared.DTOs.Request;
using Shared.DTOs.Response;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Utilities.Constants;
using Utilities.Enum;

namespace Service
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly UserManager<UserModel> _userManager;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private UserModel? _user;
        

        public AuthenticationService(
            ILoggerManager logger,
            IMapper mapper,
            UserManager<UserModel> userManager,
            IOptions<JwtConfiguration> configuration,
            RoleManager<IdentityRole> roleManager,
            IRepositoryManager repository, IEmailService emailService
            )
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
            _jwtConfiguration = configuration.Value;
            _roleManager = roleManager;
            _repository = repository;
            _emailService = emailService;



        }
        public Task<IdentityResult> ActivateAccount(AccountActivationByEmailDto accountActivation)
        {
            throw new NotImplementedException();
        }

        private protected async Task<TokenDto> CreateToken(bool populateExp, string deviceId)
        {
            // Get the signing credentials used to sign the token
            var signingCredentials = GetSigningCredentials();

            // Get the claims associated with the authenticated user
            var claims = await GetClaims();

            // Generate the options for the JWT token
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();

            //if (populateExp)
            //    _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(Convert.ToDouble(_jwtConfiguration.rExpires));
            var refreshTokenExpiryTime = DateTime.Now.AddDays(Convert.ToDouble(_jwtConfiguration.rExpires));
           
            var sessioninfo = await CreateLoginSessionAsync(refreshToken, refreshTokenExpiryTime, deviceId);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            // Write the token as a string
            return new TokenDto() { AccessToken = accessToken, RefreshToken = refreshToken, DeviceId = sessioninfo.DeviceId };
        }

        private protected async Task<TokenDto> CreateTokenRefresh(bool populateExp, LoginSessions session)
        {
            // Get the signing credentials used to sign the token
            var signingCredentials = GetSigningCredentials();

            // Get the claims associated with the authenticated user
            var claims = await GetClaims();

            // Generate the options for the JWT token
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);


            //if (populateExp)
            //    _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(Convert.ToDouble(_jwtConfiguration.rExpires));
            //var refreshTokenExpiryTime = DateTime.Now.AddDays(Convert.ToDouble(_jwtConfiguration.rExpires));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            // Write the token as a string
            return new TokenDto() { AccessToken = accessToken, RefreshToken = session.RefreshToken, DeviceId = session.DeviceId };
        }

        private async Task<LoginSessions> CreateLoginSessionAsync (string refreshToken, DateTime expiryDate, string deviceId)
        {
            try
            {
                LoginSessions? prevSession = null;
                if (deviceId != null)
                {
                    prevSession = await _repository.LoginSession.GetLoginSessionByDeviceId(new (deviceId), true);
                }

                if (prevSession != null)
                {
                    prevSession.RefreshToken = refreshToken;
                    prevSession.ExpirationDate = expiryDate;
                    prevSession.ToUpdate();
                    
                } else
                {
                    prevSession = new LoginSessions()
                    {
                        UserId = _user.Id,
                        RefreshToken = refreshToken,
                        ExpirationDate = expiryDate,
                    };

                    _repository.LoginSession.CreateLoginSession(prevSession);
                }
                await _repository.SaveAsync();
                return prevSession;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new BadRequestException("Error while creating session");
            }
        }

        public Task<string> GetEmailConfirmationToken(UserModel user)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var session = await _repository.LoginSession.GetLoginSessionByDeviceId(tokenDto.DeviceId, false);

            var user = await _userManager.FindByEmailAsync(principal.Identity.Name);
            if (user == null || session == null || session.RefreshToken != tokenDto.RefreshToken || session.CheckExpiry())
            {
                throw new RefreshTokenBadRequest();
            }
            _user = user;

            return await CreateTokenRefresh(populateExp: false, session);
        }

        public async Task<TokenDto> RefreshToken2(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var session = await _repository.LoginSession.GetLoginSessionByDeviceId(tokenDto.DeviceId, false);

            var user = await _userManager.FindByEmailAsync(principal.Identity.Name);
            if (user == null || session == null || session.RefreshToken != tokenDto.RefreshToken || session.CheckExpiry())
            {
                throw new RefreshTokenBadRequest();
            }
            _user = user;

            return await CreateTokenRefresh(populateExp: false, session);
        }


        public async Task<IdentityResult> RegisterAdminUser(UserAdminRegistrationDto userAdminRegistration)
        {
            ValidateLicense(userAdminRegistration.LicenseCode);

            var user = _mapper.Map<UserModel>(userAdminRegistration);
            // email confirmation create

            var result = await _userManager.CreateAsync(user, userAdminRegistration.Password);

            var roles = new List<string>() { "Admin" };

            if (result.Succeeded)
            {
                // filter the roles that exist in the role manager
                var validRoles = roles
                  .Where(role => _roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                  .ToList();
                Guid TemUser = await TaskSyncTempAndAdminUser(user.Email, user, validRoles[0]);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // Add user to the roles and user join table
                await _userManager.AddToRolesAsync(user, validRoles);
                _emailService.CreateEmail(userAdminRegistration.Email, TemUser, code, EmailTypeEnums.AccountActivation);
            }

            return result;
        }

        public async Task<IdentityResult> RegisterNormalUser(NormalUserRegistrationDto normalUser)
        {
            var email = ValidateNormalUser(normalUser.Token);
            var tempUser = await _repository.TempUser.GetTempUser(email, false);
            if (tempUser == null)
            {
                _logger.LogWarn(string.Format(ErrorMessage.ObjectNotFound, "User"));
                throw new InvalidUserException();
            }

            var user = _mapper.Map<UserModel>(normalUser);
            user.Email = email;
            var result = await _userManager.CreateAsync(user, normalUser.Password);

            var roles = new List<string>() { "Staff" };

            if (result.Succeeded)
            {
                // filter the roles that exist in the role manager
                var validRoles = roles
                  .Where(role => _roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                  .ToList();
                // Add user to the roles and user join table
                await _userManager.AddToRolesAsync(user, validRoles);
                _emailService.CreateEmail(user.Email, tempUser.Id, null, emailType: EmailTypeEnums.NewAccount);
                await SyncTempUserAndUserAsync(email, user, validRoles[0]);
            }
            return result;
        }

        public async Task<IAuthResponse> ValidateUser(UserForAuthenticationDto userForAuth, string? DeviceId)
        {
            _user = await _userManager.FindByEmailAsync(userForAuth.Email);


            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuth.Password));


            if (!result)
                throw new InvalidCredentialsException();
            if (!_user.IsActive)
            {
                throw new ActivateUserException();
            }
            if (await _userManager.GetTwoFactorEnabledAsync(_user))
            {
                // Send 2FA code or direct user to verify 2FA
                return new TwoFactorResponse()
                {
                    Requires2FA = true,
                    UserId = _user.Id,
                    Email = _user.Email
                };
            }


            return await CreateToken(populateExp: true, DeviceId);
        }



        public async Task<TokenDto> Verify2fa(Verify2faDto model, string DeviceId)
        {
            _user = await _userManager.FindByEmailAsync(model.Email);
            if (_user == null)
            {
                throw new NotFoundException("User not found");
            }

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
           _user, TokenOptions.DefaultAuthenticatorProvider, model.Code);

            if (!is2faTokenValid)
            {
                throw new InvalidCodeException("Invalid verification code.");
            }
            if (!_user.IsActive)
            {
                throw new ActivateUserException();
            }

            return await CreateToken(populateExp: true, DeviceId);
        }

        private protected void ValidateLicense(string code)
        {
            if (code != License.Code)
            {
                _logger.LogWarn(ErrorMessage.InvalidLicense);
                throw new InvalidLicenseException(ErrorMessage.InvalidLicense);
            }
        }

        // Get signing credentials for JWT token
        private static SigningCredentials GetSigningCredentials()
        {
            // Get the secret key from the environment variable
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(Constants.SecretKey));

            // Create a new symmetric security key using the secret key
            var secret = new SymmetricSecurityKey(key);

            // Return the signing credentials using the symmetric security key
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        // Get claims for JWT token
        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                // Add the user's name as a claim
                new Claim(ClaimTypes.Name, value: _user.Email),
                new Claim(type: JwtRegisteredClaimNames.Sub, value: _user.Id),
                new Claim(type: JwtRegisteredClaimNames.Email, value: _user.Email),
                new Claim(type: JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(type: JwtRegisteredClaimNames.Iat, value: DateTime.Now.ToUniversalTime().ToString())
                //new Claim(ClaimTypes.Email, _user.Email),
            };

            // Get the roles associated with the user
            var roles = await _userManager.GetRolesAsync(_user);

            // Add each role as a claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(
           SigningCredentials signingCredentials,
           List<Claim> claims)
        {
            // Get the JWT settings from the configuration
            var datet = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires));
            // Create a new JWT security token with the specified options
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: datet,
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }
        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }

        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {

            // Set up token validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(Constants.SecretKey))),
                ValidateLifetime = false,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            };

            // Create a new JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Validate the token and retrieve the principal
            SecurityToken securityToken;
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

                // Check if the token is a valid JwtSecurityToken
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Throw an exception if the token is invalid
                    throw new RefreshTokenBadRequest();
                }

                // Return the principal extracted from the token
                return principal;
            }
            catch (Exception ex)
            {
                throw new RefreshTokenBadRequest();
            }
        }

        public UserRegTokenDto CreateUserRegCode(string email)
        {
            return new UserRegTokenDto()
            {
                regCode = GenerateNewUserJwtToken(email)
            };
        }

        private string GenerateNewUserJwtToken(string userEmail)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(userEmail);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            // Write the token as a string
            return accessToken;

        }

        private static List<Claim> GetClaims(string email)
        {
            var claims = new List<Claim>
            {
                // Add the user's name as a claim
                new Claim(ClaimTypes.Email, email)
            };

            return claims;
        }

        private string ValidateNormalUser(string token)
        {
            var principalAndEmail = GetPrincipalFromToken(token);
            if (principalAndEmail.Item1 == null || principalAndEmail.Item2 == null)
            {
                throw new SecurityTokenException(string.Format(Constants.InvalidSubject, Constants.Token));
            }
            return principalAndEmail.Item2;

        }
        private Tuple<ClaimsPrincipal, string> GetPrincipalFromToken(string token)
        {

            // Set up token validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(Constants.SecretKey))),
                ValidateLifetime = true,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            };

            // Create a new JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Validate the token and retrieve the principal
            SecurityToken securityToken;
            ClaimsPrincipal? principal;
            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            }
            catch (Exception ex)
            {
                _logger.LogInfo($"token expired");
                throw new ExpiredToken();

            }
            var jwtSecurityTokenUE = tokenHandler.ReadJwtToken(token);
            var claims = jwtSecurityTokenUE.Claims;
            var emailClaim = claims.First();

            if (emailClaim == null)
            {
                throw new SecurityTokenException(string.Format(Constants.InvalidSubject, Constants.Token));
            }


            // Check if the token is a valid JwtSecurityToken
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                // Throw an exception if the token is invalid
                throw new SecurityTokenException(string.Format(Constants.InvalidSubject, Constants.Token));
            }
            if (principal == null)
            {
                throw new SecurityTokenException(string.Format(Constants.InvalidSubject, Constants.Token));
            }

            // Return the principal extracted from the token
            return Tuple.Create(principal, emailClaim.Value);
        }

        private async Task SyncTempUserAndUserAsync(string email, UserModel user, string roles)
        {
            var tempUser = await _repository.TempUser.GetTempUser(email, true);
            if (tempUser == null)
            {
                _logger.LogWarn(string.Format(ErrorMessage.ObjectNotFound, "User"));
                throw new NotFoundException(string.Format(ErrorMessage.ObjectNotFound, "User"));
            }
            tempUser.UserId = user.Id;
            tempUser.IsActive = true;
            tempUser.Role = roles;
            await _repository.SaveAsync();
        }
        private async Task<Guid> TaskSyncTempAndAdminUser(string email, UserModel user, string roles)
        {
            var tempUser = await _repository.TempUser.GetTempUser(email, true);
            if (tempUser != null)
            {
                _logger.LogWarn("user already exists");

            }
            var _tempUser = new TempUserModel()
            {
                Email = email,
                IsActive = true,
                UserId = user.Id,
                Role = roles

            };
            _repository.TempUser.CreateTempUser(_tempUser);
            await _repository.SaveAsync();
            return _tempUser.Id;
        }

        public async Task ForgetPassword(NewUserAddDto user)
        {
            var tempUser = await _repository.TempUser.GetTempUser(user.Email, false);
            if (tempUser.UserModel != null)
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(tempUser.UserModel);
                _emailService.CreateEmail(user.Email, tempUser.Id, code, EmailTypeEnums.ResetPassword);
            }
        }

        public async Task<IdentityResult> ChangePassword(ForgetPasswordDto forgetPassword)
        {
            try
            {
                var tempUser = await _repository.TempUser.GetTempUser(forgetPassword.UserId, true);
                var result = await _userManager.ResetPasswordAsync(tempUser.UserModel, forgetPassword.Token, forgetPassword.Password);

                if (result.Succeeded)
                {
                    _emailService.CreateEmail(tempUser.Email, tempUser.Id, forgetPassword.Token, EmailTypeEnums.ChangePassword);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new IdentityResult();
            }
        }

        public async Task<MFAKeyURLDto> Enrole2FA(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            var key = FormatKey(unformattedKey);
            var uri = GenerateQrCodeUri(user.Email, unformattedKey);

            return new MFAKeyURLDto()
            {
                key = key,
                uri = uri
            };
        }


        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                $"otpauth://totp/{_jwtConfiguration.ValidIssuer}:{email}?secret={unformattedKey}&issuer={_jwtConfiguration.ValidIssuer}&digits=6"
                );
        }

        public async Task VerifyAuthenticator(string userId, VerifyAuthenticatorDto authenticatorDto)
        {
            var user = await UserExist(userId);

            var verificationCode = authenticatorDto.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
            user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
            if (!is2faTokenValid)
            {
                throw new Exception("Invalid verification code.");
            }
            await _userManager.SetTwoFactorEnabledAsync(user, true);
            return;
        }
        private async Task<UserModel> UserExist(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new NotFoundException("User not Found");
            }
            return user;
        }
        public async Task Disable2faAsync(string userId)
        {
            var user = await UserExist(userId);

            var disableResult = await _userManager.SetTwoFactorEnabledAsync(user, false);

            if (!disableResult.Succeeded) { 
                throw new BadRequestException("Unable to diable 2FA");
            }
            return;
        }   

        
    }

}
