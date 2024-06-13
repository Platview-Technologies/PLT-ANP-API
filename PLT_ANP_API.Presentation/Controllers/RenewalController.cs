using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLT_ANP_API.Presentation.ActionFilters;
using Service.Contract;
using Shared.DTOs.Request;
using System.Text.Json;

namespace PLT_ANP_API.Presentation.Controllers
{
    [Route("api/renewal")]
    [ApiController]
    [Authorize]
    public class RenewalController : ControllerBase
    {
        private readonly IServiceManager _service;

        public RenewalController(IServiceManager service)
        {
            _service = service;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateRenewal([FromBody] RenewalRequestDto renewalRequest)
        {
            var Renewal = await _service.RenewalService.CreateRenewal(renewalRequest);
            return CreatedAtRoute("GetRenewalById", new { id = Renewal.Id }, Renewal);
        }

        [HttpGet("{Id:Guid}", Name = "GetRenewalById")]
        public async Task<IActionResult> GetRenewal(Guid Id)
        {
            var Renewal = await _service.RenewalService.GetRenewalAsync(Id);
            return Ok(Renewal);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRenewals()
        {
            var renewals = await _service.RenewalService.GetAllRenewalAsync();
            return Ok(renewals);
        }

        [HttpGet("page/{Page:int}")]
        public async Task<IActionResult> GetRenewalsPaginated(int page)
        {
            if (page < 1)
            {
                return BadRequest("Page number must be greater than or equal to 1.");
            }
            var (renewals, metaData) = await _service.RenewalService.GetRenewalsPaginated(page);

            // Serialize pagination metadata to JSON and add to response headers
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metaData));

            return Ok(renewals);
        }

        [HttpDelete("{Id:Guid}")]
        public async Task<IActionResult> DeleteRenewal(Guid Id)
        {
            await _service.RenewalService.DeleteRenewal(Id);
            return NoContent();
        } 
    }
}
