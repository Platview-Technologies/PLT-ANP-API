﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLT_ANP_API.Presentation.ActionFilters;
using Service.Contract;
using Shared.DTOs;
using Shared.DTOs.Response;

namespace PLT_ANP_API.Presentation.AuthenticationController
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IServiceManager _service;

        public TokenController(IServiceManager service)
        {
            _service = service;
        }

        [HttpPost("refresh")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
        {
            var returnToken = await _service.AuthenticationService.RefreshToken(tokenDto);
            return Ok(returnToken);                               
        }

        [HttpPost("refreshCookie")]
        
        public async Task<IActionResult> RefreshTokenCookie()
        {
            string refreshToken = Request.Cookies["rt"];
            string accessToken = Request.Cookies["aT"];
            if (refreshToken == null || accessToken == null)
            {
                return Unauthorized();
            }
            var tokenDto = new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var returnToken = await _service.AuthenticationService.RefreshToken(tokenDto);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Make sure to set this to true if using HTTPS
                SameSite = SameSiteMode.None, // Adjust as per your requirements
                MaxAge = TimeSpan.FromDays(7), // Set the cookie's expiration time
                Expires = DateTime.Now.AddDays(7)
            };

            Response.Cookies.Append("rt", returnToken.RefreshToken, cookieOptions);
            Response.Cookies.Append("aT", returnToken.AccessToken, cookieOptions);
            return Ok(new ATokenDto() { AccessToken = returnToken.AccessToken });
        }
    }
}
