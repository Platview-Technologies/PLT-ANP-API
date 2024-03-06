using Entities.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLT_ANP_API.Presentation.ActionFilters;
using Service.Contract;
using Shared.DTOs.Request;
using Shared.DTOs.Response;
using System.Diagnostics.Contracts;
using Utilities.Constants;

namespace PLT_ANP_API.Presentation.AuthenticationController
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController: ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly IEmailService _emailService;

        public AuthenticationController(IServiceManager service, IEmailService emailService)
        {
            _service = service;
            _emailService = emailService;
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


        
    }
}
