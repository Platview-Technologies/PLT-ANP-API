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
using Service.Contract;
using Shared.DTOs;
using Shared.DTOs.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Utilities.Constants;

namespace Service
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
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
            IRepositoryManager repository)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
            _jwtConfiguration = configuration.Value;
            _roleManager = roleManager;
            _repository = repository;

                
        }
        public Task<IdentityResult> ActivateAccount(AccountActivationByEmailDto accountActivation)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenDto> CreateToken(bool populateExp)
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

        public Task<TokenDto> RefreshToken(TokenDto tokenDto)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> RegisterAdminUser(UserAdminRegistration userAdminRegistration)
        {
            ValidateLicense(userAdminRegistration.LicenseCode);

            var user = _mapper.Map<UserModel>(userAdminRegistration);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // email confirmation create

            var result = await _userManager.CreateAsync(user, userAdminRegistration.Password);

            var roles = new List<string>() { "Admin" };

            if (result.Succeeded)
            {
                // filter the roles that exist in the role manager
                var validRoles = roles
                  .Where(role => _roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                  .ToList();

                // Add user to the roles and user join table
                await _userManager.AddToRolesAsync(user, validRoles);
            }

            return result;
        }

        public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth)
        {
            _user = await _userManager.FindByEmailAsync(userForAuth.Email);


            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, userForAuth.Password));

            if (!_user.IsActive)
            {
                throw new ActivateUserException();
            }

            if (!result)
                throw new InvalidCredentialsException();
            
            return result;
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
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"));

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

    }
}
