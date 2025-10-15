using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPark.Common.Wrapper;
using SmartPark.CQRS.Commands.Billing;
using SmartPark.CQRS.Queries.Billing;
using SmartPark.Dtos.Billing;

namespace SmartPark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class BillingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BillingController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("create-billing")]

        public async Task<IActionResult> CreateAsync([FromBody] BillingRequest dto)
        {
            var result = await _mediator.Send(new CreateBillingCommand(dto));
            return Ok(new ApiResponse<BillingResponse>
            {
                Success = true,
                Message = "Billing created successfully",
                Data = result
            });
        }

        [HttpGet("get-billing-by/{id:guid}")]
        [ProducesResponseType(typeof(BillingDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var result = await _mediator.Send(new GetBillingByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("get-all-billings")]
        [ProducesResponseType(typeof(IEnumerable<BillingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _mediator.Send(new GetAllBillingsQuery());
            return Ok(result);
        }

        [HttpPut("update-billing/{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] BillingRequest dto)
        {
            var result = await _mediator.Send(new UpdateBillingCommand(id, dto));
            return Ok(new ApiResponse<BillingResponse>
            {
                Success = true,
                Message = "Billing updated successfully",
                Data = result
            });
        }

        [HttpDelete("delete-billing/{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var success = await _mediator.Send(new DeleteBillingCommand(id));
            if (!success) return NotFound();
            return Ok(new { Message = "Billing deleted successfully" });
        }

    }
}
