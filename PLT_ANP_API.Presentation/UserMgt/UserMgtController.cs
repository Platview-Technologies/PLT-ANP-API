using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLT_ANP_API.Presentation.ActionFilters;
using Service.Contract;
using Shared.DTOs.Request;
using Shared.DTOs.Response;
using Utilities.Enum;

namespace PLT_ANP_API.Presentation.UserMgt
{
    [ApiController]
    [Route("api/usermgt")]
    [Authorize(Roles = "Admin")]
    public class UserMgtController : ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly IEmailService _emailService;

        public UserMgtController(IServiceManager service, IEmailService emailService)
        {
            _service = service;
            _emailService = emailService;
        }

        [HttpPost()]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status400BadRequest)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> AddNewUser([FromBody] NewUserAddDto emailUser)
        {
            Guid TempUserId = await _service.UserManagementService.CreateTempUser(emailUser.Email);
            var Token = _service.AuthenticationService.CreateUserRegCode(emailUser.Email);
            _emailService.CreateEmail(email: emailUser.Email, tempUserId: TempUserId, token: Token.regCode, emailType: EmailTypeEnums.UserRegistration);
            var _ = new TempUserResponseDto()
            {
                Id = TempUserId
            };

            return Ok(_);
        }

        [HttpGet()]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _service.UserManagementService.GetAllTempUser();
            return Ok(users);
        }

        [HttpDelete("{Id:Guid}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteUser(Guid Id)
        {
            await _service.UserManagementService.DeleteUser(Id);
            return NoContent();
        }
    }
}
