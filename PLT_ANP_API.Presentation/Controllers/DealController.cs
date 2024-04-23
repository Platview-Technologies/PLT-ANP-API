using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLT_ANP_API.Presentation.ActionFilters;
using Service.Contract;
using Shared.DTOs.Request;
using System.Text.Json;


namespace PLT_ANP_API.Presentation.Controllers
{
    [Route("api/deals")]
    [ApiController]
    [Authorize]
    public class DealController : ControllerBase
    {
        private readonly IServiceManager _service;
        public DealController(IServiceManager service)
        { 
            _service = service;                
        }

        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetDeals()
        {
            var deals = await _service.DealService.GetDeals(false);
            return Ok(deals);
        }
        [HttpGet("page/{Page:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(string), 400)] // Example of a bad request response
        public async Task<IActionResult> GetDeals(int page)
        {
            if (page < 1)
            {
                return BadRequest("Page number must be greater than or equal to 1.");
            }

            var dealsPagedResult = await _service.DealService.GetDeals(false, page);


            // Serialize pagination metadata to JSON and add to response headers
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(dealsPagedResult.metaData));

            return Ok(dealsPagedResult.deals);
        }

        [HttpGet("{Id:Guid}", Name ="GetDealById")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDeal(Guid Id)
        {
            var deal = await _service.DealService.GetDeal(Id, false);
            return Ok(deal);

        }


        [HttpPost(Name = "CreatedDeal")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateDeal([FromBody] DealRequestDto deal)
        {
            var createdDeal = await _service.DealService.CreateDeal(deal, false);
            return CreatedAtRoute("GetDealById", new { id = createdDeal.Id }, createdDeal);
        }

        [HttpPut("{Id:Guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateDeal([FromBody] DealUpdateDto dealUpdate, Guid Id)
        {
            await _service.DealService.UpdateDeal(Id, dealUpdate, true);
            return NoContent();
        }

        [HttpDelete("{Id:Guid}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteDeal(Guid Id)
        {
            await _service.DealService.DeleteDeals(Id, true);
            return NoContent();
        }


    }
}
