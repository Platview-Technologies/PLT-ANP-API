using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using Shared.DTOs;
using Shared.DTOs.Response;
using System.Security.Claims;


namespace PLT_ANP_API.Presentation.AuthenticationController
{
    
    [ApiController]
    [Route("api/mfa")]
    public class TwoFactorController : ControllerBase
    {
        private readonly IServiceManager _services;

        public TwoFactorController(IServiceManager services)
        {
            _services = services;
        }
        [Authorize]
        [HttpGet("enable-authenticator")]
        public async Task<IActionResult> EnableAuthenticator()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var mfaInfo = await _services.AuthenticationService.Enrole2FA(userId);
            return Ok(mfaInfo);
        }
        [Authorize]
        [HttpPost("verify-authenticator")]
        public async Task<IActionResult> VerifyAuthenticator([FromBody] VerifyAuthenticatorDto model)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _services.AuthenticationService.VerifyAuthenticator(userId, model);
            return Ok();

        }

        [HttpPost("VerifyMFAUser")]
        public async Task<IActionResult> VeryfyMFAUser([FromBody] Verify2faDto model)
        {
            var tokenResponse = await _services.AuthenticationService.Verify2fa(model);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Make sure to set this to true if using HTTPS
                SameSite = SameSiteMode.None, // Adjust as per your requirements
                MaxAge = TimeSpan.FromDays(7), // Set the cookie's expiration time
                Expires = DateTime.Now.AddDays(7),
            };

            
            Response.Cookies.Append("rt", tokenResponse.RefreshToken, cookieOptions);
            Response.Cookies.Append("aT", tokenResponse.AccessToken, cookieOptions);
            return Ok(new ATokenDto() { AccessToken = tokenResponse.AccessToken });
            
        }
    }
}
