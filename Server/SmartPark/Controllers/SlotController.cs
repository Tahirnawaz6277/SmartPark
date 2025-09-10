//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using SmartPark.CQRS.Commands.Slot;
//using SmartPark.CQRS.Queries.Slot;
//using SmartPark.Dtos.Slot;

//namespace SmartPark.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    //[Authorize(Roles = "Admin")]
//    public class SlotController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public SlotController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        [HttpPost("add-slot")]
//        public async Task<IActionResult> CreateAsync([FromBody] SlotRequestDto dto)
//        {
//            var result = await _mediator.Send(new CreateSlotCommand(dto));
//            return Ok(result);
//        }


//        [HttpGet("get-slot-by/{id:guid}")]
//        public async Task<IActionResult> GetByIdAsync(Guid id)
//        {
//            var result = await _mediator.Send(new GetSlotByIdQuery(id));
//            if (result == null) return NotFound();
//            return Ok(result);
//        }

//        [HttpGet("get-all-slots")]
//        public async Task<IActionResult> GetAllAsync()
//        {
//            var result = await _mediator.Send(new GetAllSlotsQuery());
//            return Ok(result);
//        }

//        [HttpPut("update-slot/{id:guid}")]
//        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] SlotRequestDto dto)
//        {
//            var result = await _mediator.Send(new UpdateSlotCommand(id, dto));
//            return Ok(result);
//        }

//        [HttpDelete("delete-slot/{id:guid}")]
//        public async Task<IActionResult> DeleteAsync(Guid id)
//        {
//            var success = await _mediator.Send(new DeleteSlotCommand(id));
//            if (!success) return NotFound();
//            return Ok(new { Message = "Slot deleted successfully" });
//        }
//    }
//}

