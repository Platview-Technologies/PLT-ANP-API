using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PLT_ANP_API.Presentation.ActionFilters;
using Service.Contract;
using Shared.DTOs.Request;


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

        [HttpGet("{Id:Guid}", Name ="GetDealById")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDeal(Guid Id)
        {
            var deal = await _service.DealService.GetDeal(Id, false);
            return Ok(deal);

        }


        [HttpPost(Name = "CreatedProject")]
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
            await _service.DealService.DeleteProject(Id, true);
            return NoContent();
        }


    }
}
