using Entities.ConfigurationModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLT_ANP_API.Presentation.ActionFilters;
using Service.Contract;
using Shared.DTOs;
using Shared.DTOs.Request;
using Shared.DTOs.Response;
using System.Security.Claims;
using Utilities.Constants;

namespace PLT_ANP_API.Presentation.AuthenticationController
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController: ControllerBase
    {
        private readonly IServiceManager _service;

        public AuthenticationController(IServiceManager service, IEmailService emailService)
        {
            _service = service;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserAdminRegistrationDto userAdmin)
        {
            var result = await _service.AuthenticationService.RegisterAdminUser(userAdmin);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            
            NewUserDto _ = new() { message = Constants.NewAccountMessage };
            return Ok(_);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
        {
            var userResponse = await _service.AuthenticationService.ValidateUser(user);
            return Ok(userResponse);
        }

        [HttpPost("loginCookie")]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status401Unauthorized)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> AuthenticateCookie([FromBody] UserForAuthenticationDto user)
        {
            var userResponse = await _service.AuthenticationService.ValidateUser(user);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Make sure to set this to true if using HTTPS
                SameSite = SameSiteMode.None, // Adjust as per your requirements
                MaxAge = TimeSpan.FromDays(7), // Set the cookie's expiration time
                Expires = DateTime.Now.AddDays(7),
                
            };
           
            if (userResponse is TokenDto tokenResponse)
            {

            Response.Cookies.Append("rt", tokenResponse.RefreshToken, cookieOptions);
            Response.Cookies.Append("aT", tokenResponse.AccessToken, cookieOptions);
            return Ok(new ATokenDto() {AccessToken = tokenResponse.AccessToken });
            }else
            {
                return Ok(userResponse);
            }
        }

        [HttpPost("RegisterUser")]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterNormalUser(NormalUserRegistrationDto normalUser)
        {
            var result = await _service.AuthenticationService.RegisterNormalUser(normalUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            
            NewUserDto _ = new() { message = Constants.NewAccountNormalUser };
            return Ok(_);

        }

        [HttpPost("ForgetPassword")]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ForgetPassword(NewUserAddDto userEmail)
        {
            await _service.AuthenticationService.ForgetPassword(userEmail);
            return NoContent();
        }

        [HttpPost("ChangePassword")]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> ChangePassword(ForgetPasswordDto forgetPassword)
        {
            var result = await _service.AuthenticationService.ChangePassword(forgetPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok();
        }

        [HttpGet("GetUser")]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status400BadRequest)]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            string userId = GetUserId();
            var user = await _service.UserManagementService.GetUser(userId);
            return Ok(user);
        }


        [HttpGet("IsAuth")]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<IActionResult> IsAuth()
        {
            return Ok(DateTime.Now.AddMinutes(Convert.ToDouble(1)));
        }

        private protected string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }
        

    }
}
