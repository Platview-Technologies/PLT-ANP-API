using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using System.Text.Json;

namespace PLT_ANP_API.Presentation.Controllers
{
    [Route("api/notification")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly INotificationService _notification;

        public NotificationsController(IServiceManager service, INotificationService notification)
        {
            _service = service;
            _notification = notification;
        }

        [HttpGet("page/{Page:int}")]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> getAllNotifications(int page)
        {
            if (page < 1)
            {
                return BadRequest("Page number must be greater than or equal to 1.");
            }

            var notifications = await _notification.GetNotifications(false, page);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(notifications.metaData));
            return Ok(notifications.notifications);
        }

        [HttpGet("{Id:Guid}")]
        public async Task<IActionResult> GetNotification(Guid Id)
        {
            var notification = await _notification.GetNotification(Id, false);
            return Ok(notification);
        }
        
    }
}
