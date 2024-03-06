using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PLT_ANP_API.Presentation.ActionFilters;
using Service.Contract;
using Shared.DTOs.Request;
using Utilities.Enum;

namespace PLT_ANP_API.Presentation.UserMgt
{
    [ApiController]
    [Route("api/usermgt")]
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
        [Authorize("Admin")]
        public async Task<IActionResult> AddNewUser([FromBody] NewUserAddDto emailUser)
        {
            Guid TempUserId = _service.UserManagementService.CreateTempUser(emailUser.Email);
            var Token = _service.AuthenticationService.CreateUserRegCode(emailUser.Email);
            _emailService.CreateEmail(email: emailUser.Email, userId: TempUserId, token: Token.regCode, emailType: EmailTypeEnums.UserRegistration);
            // remember to do this in the userMangement Services
            return Ok();
        }

        [HttpGet()]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status400BadRequest)]
        [Authorize("Admin")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _service.UserManagementService.GetAllTempUser();
            return Ok(users);
        }

        [HttpDelete("{Id:Guid}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            _service.UserManagementService.DeleteUser(id);
            return NoContent();
        }

    }
}
