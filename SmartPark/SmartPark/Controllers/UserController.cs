using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartPark.Common.Wrapper;
using SmartPark.CQRS.Commands;
using SmartPark.Dtos;

namespace SmartPark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("user-registration")]
        public async Task<IActionResult> Create([FromBody] UserRequestDto requestDto)
        {
            var command = new CreateUserCommand(requestDto);
            var user = await _mediator.Send(command);
            return Ok(new ApiResponse<UserResponseDto> 
            { 
                Success = true,
                Message = "User registered successfully ",
                Data = user 
            });
        }

        //[HttpGet("get-user-by/{id}")]
        //public async Task<IActionResult> GetById(Guid id)
        //{
        //    var user = await _mediator.Send(new GetUserByIdQuery(id));
        //    return user != null ? Ok(user) : NotFound();
        //}
    }
}
