using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPark.Common.Wrapper;
using SmartPark.CQRS.Commands.Location;
using SmartPark.CQRS.Queries.Location;
using SmartPark.Dtos.Location;

namespace SmartPark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-location")]
        public async Task<IActionResult> CreateAsync([FromForm] LocationRequest dto)
        {
            var result = await _mediator.Send(new CreateLocationCommand(dto));
            return Ok(new ApiResponse<LocationReponse>
            {
                Success = true,
                Message = "Location created successfully with slots",
                Data = result
            });
        }

        [Authorize(Roles = "Driver,Admin")]
        [HttpGet("get-location-by/{id}")]
        [ProducesResponseType(typeof(LocationDto), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _mediator.Send(new GetLocationByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }


        //[Authorize(Roles = "Admin")]
        //[HttpPost("location")]
        //public async Task<IActionResult> UploadLocation([FromForm] Guid locationId, [FromForm] IFormFile file)
        //{
        //    var result = await _mediator.Send(new UploadLocationImageCommand { LocationId = locationId, File = file });
        //    return Ok(new { path = result });
        //}


        [Authorize(Roles = "Driver,Admin")]
        [HttpGet("get-slots-by/{locationId}")]
        [ProducesResponseType(typeof(SlotResponseDto), StatusCodes.Status200OK)]

        public async Task<IActionResult> GetSlotsByLocationIdAsync(Guid locationId)
        {
            var result = await _mediator.Send(new GetSlotsByLocationIdQuery(locationId));
            return Ok(result);
        }

        [Authorize(Roles = "Driver,Admin")]
        [HttpGet("get-all-locations")]
        [ProducesResponseType(typeof(IEnumerable<LocationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _mediator.Send(new GetAllLocationsQuery());
            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("update-location/{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromForm] UpdateLocationRequest dto)
        {
            var result = await _mediator.Send(new UpdateLocationCommand(id, dto));
            return Ok(new ApiResponse<LocationReponse>
            {
                Success = true,
                Message = "Location update successfully with slots",
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-location/{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var success = await _mediator.Send(new DeleteLocationCommand(id));
            if (!success) return NotFound();
            return Ok(new { Message = "Location deleted successfully" });
        }


    }
}
