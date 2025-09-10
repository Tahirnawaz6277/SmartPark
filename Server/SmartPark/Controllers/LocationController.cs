using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPark.CQRS.Commands.Location;
using SmartPark.Dtos.Location;

namespace SmartPark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("create-location")]
        public async Task<IActionResult> CreateAsync([FromBody] LocationRequest dto)
        {
            var result = await _mediator.Send(new CreateLocationCommand(dto));
            return Ok(result);
        }

        ////[Authorize(Roles = "Driver,Admin")]
        //[HttpGet("get-location-by/{id}")]
        //public async Task<IActionResult> GetByIdAsync(Guid id)
        //{
        //    var result = await _mediator.Send(new GetLocationByIdQuery(id));
        //    if (result == null) return NotFound();
        //    return Ok(result);
        //}

        ////[Authorize(Roles = "Driver,Admin")]
        //[HttpGet("get-all-locations")]
        //public async Task<IActionResult> GetAllAsync()
        //{
        //    var result = await _mediator.Send(new GetAllLocationsQuery());
        //    return Ok(result);
        //}

        //[HttpPut("update-location/{id:guid}")]
        //public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CreateLocationRequest dto)
        //{
        //    var result = await _mediator.Send(new UpdateLocationCommand(id, dto));
        //    return Ok(result);
        //}

        [HttpDelete("delete-location/{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var success = await _mediator.Send(new DeleteLocationCommand(id));
            if (!success) return NotFound();
            return Ok(new { Message = "Location deleted successfully" });
        }


    }
}
