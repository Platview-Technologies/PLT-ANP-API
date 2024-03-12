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

        private protected async Task<TokenDto> CreateToken(bool populateExp)
        {
            // Get the signing credentials used to sign the token
            var signingCredentials = GetSigningCredentials();

            // Get the claims associated with the authenticated user
            var claims = await GetClaims();

            // Generate the options for the JWT token
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            var refreshToken = GenerateRefreshToken();

            _user.RefreshToken = refreshToken;
            
            if (populateExp)
                _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            // Write the token as a string
            return new TokenDto(accessToken, refreshToken);
        }

        public Task<string> GetEmailConfirmationToken(UserModel user)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

            var user = await _userManager.FindByEmailAsync(principal.Identity.Name);
            if (user == null || user.RefreshToken != tokenDto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new RefreshTokenBadRequest();

            _user = user;

            return await CreateToken(populateExp: false);
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
                Guid TemUser = await TaskSyncTempAndAdminUser(user.Email, user);
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
                _emailService.CreateEmail(user.Email, tempUser.Id, null,emailType:EmailTypeEnums.NewAccount);
                await SyncTempUserAndUserAsync(email, user);
            }
            return result;
        }

        public async Task<TokenDto> ValidateUser(UserForAuthenticationDto userForAuth)
        {
            _user = await _userManager.FindByEmailAsync(userForAuth.Email);


            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuth.Password));

            if (!_user.IsActive)
            {
                throw new ActivateUserException();
            }

            if (!result)
                throw new InvalidCredentialsException();

            return await CreateToken(populateExp: true);
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
                new Claim(ClaimTypes.Name, _user.UserName)
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

            // Create a new JWT security token with the specified options
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtConfiguration.ValidIssuer,
                audience: _jwtConfiguration.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(Constants.Secret))),
                ValidateLifetime = false,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            };

            // Create a new JwtSecurityTokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Validate the token and retrieve the principal
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            // Check if the token is a valid JwtSecurityToken
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                // Throw an exception if the token is invalid
                throw new SecurityTokenException(string.Format(Constants.InvalidSubject, Constants.Token));
            }

            // Return the principal extracted from the token
            return principal;
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

        private async Task SyncTempUserAndUserAsync(string email, UserModel user)
        {
            var tempUser = await _repository.TempUser.GetTempUser(email, true);
            if (tempUser == null)
            {
                _logger.LogWarn(string.Format(ErrorMessage.ObjectNotFound, "User"));
                throw new NotFoundException(string.Format(ErrorMessage.ObjectNotFound, "User"));
            }
            tempUser.UserId = user.Id;
            tempUser.IsActive = true;
            _repository.Save();
        }
        private async Task<Guid> TaskSyncTempAndAdminUser(string email, UserModel user)
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
                UserId = user.Id
            };
            _repository.TempUser.CreateTempUser(_tempUser);
            _repository.Save();
            return _tempUser.Id;
        }
    }
}
