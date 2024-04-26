using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLT_ANP_API.Presentation.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    public class GlobalController : ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly INotificationService _notification;

        public GlobalController(IServiceManager service, INotificationService notification)
        {
            _service = service;
            _notification = notification;
        }

        [HttpGet]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllDashboardInfo()
        {
            var dealsMeta = await _service.DealService.GetDeals(false, 1);
            var notifications = await _notification.GetNotifications(false, 1);
            var ActiveDeals = await _service.DealService.GetActiveDeals(false);
            DashboardReturnDto dashboardData = new()
            {
                ActiveDeal = ActiveDeals.Count(),
                TotalDeal = dealsMeta.metaData.TotalCount,
                Deals = dealsMeta.deals,
                Notification = notifications.notifications,
                TotalNotification = notifications.metaData.TotalCount,

            };
            return Ok(dashboardData);
            
        }

    }
}
