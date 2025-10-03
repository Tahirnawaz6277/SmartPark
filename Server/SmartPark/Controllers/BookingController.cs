using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPark.Common.Wrapper;
using SmartPark.CQRS.Commands.Booking;
using SmartPark.CQRS.Queries.Booking;
using SmartPark.Dtos.Booking;

namespace SmartPark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //  Create Booking
        [Authorize(Roles = "Admin,Driver")]
        [HttpPost("create-booking")]
        public async Task<IActionResult> CreateAsync([FromBody] BookingRequest dto)
        {
            var result = await _mediator.Send(new CreateBookingCommand(dto));
            return Ok(new ApiResponse<BookingResponse>
            {
                Success = true,
                Message = "Booking created successfully",
                Data = result
            });
        }

        // Get Booking by Id
        [Authorize(Roles = "Admin,Driver")]
        [HttpGet("get-booking-by/{id:guid}")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _mediator.Send(new GetBookingByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        //  Get All Bookings
        [HttpGet("get-all-bookings")]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _mediator.Send(new GetAllBookingsQuery());
            return Ok(result);
        }

        //  Update Booking
        [Authorize(Roles = "Admin,Driver")]
        [HttpPut("update-booking/{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BookingRequest dto)
        {
            var result = await _mediator.Send(new UpdateBookingCommand(id, dto));
            return Ok(new ApiResponse<BookingResponse>
            {
                Success = true,
                Message = "Booking updated successfully",
                Data = result
            });
        }

        // Delete Booking
        [HttpDelete("delete-booking/{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var success = await _mediator.Send(new DeleteBookingCommand(id));
            if (!success) return NotFound();
            return Ok(new { Message = "Booking deleted successfully" });
        }

        //  Get Booking Histories (by BookingId)
        [HttpGet("get-booking-histories")]
        [ProducesResponseType(typeof(IEnumerable<BookingHistoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistoriesAsync([FromQuery] Guid? bookingId)
        {
            var result = await _mediator.Send(new GetBookingHistoriesQuery(bookingId));
            return Ok(result);
        }

        //  Get Single Booking History
        [Authorize(Roles = "Admin,Driver")]
        [HttpGet("get-booking-history-by/{id:guid}")]
        [ProducesResponseType(typeof(BookingHistoryDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHistoryByIdAsync(Guid id)
        {
            var result = await _mediator.Send(new GetBookingHistoryByIdQuery(id));
            //if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
