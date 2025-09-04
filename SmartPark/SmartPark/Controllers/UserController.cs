using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartPark.Common.Wrapper;
using SmartPark.CQRS.Commands;
using SmartPark.CQRS.Queries;
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
        public async Task<IActionResult> Register([FromBody] UserRequestDto requestDto)
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
        
        
        [HttpPost("user-login")]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var query = new LoginQuery(Email,Password);
            var user = await _mediator.Send(query);
            return Ok(new ApiResponse<UserLoginResponse> 
            { 
                Success = true,
                Message = "User login successfully ",
                Data = user 
            });
        }

        [HttpGet("get-user-by/{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            //var query = new GetUserQuery(id);
            var user = await _mediator.Send(id);
            return user != null ? Ok(user) : NotFound();
        }


        //[HttpGet("get-all-users")]
        //public async Task<IActionResult> GetAllUserAsync()
        //{
        //    var query = new GetUserQuery(id);
        //    var user = await _mediator.Send();
        //    return user != null ? Ok(user) : NotFound();
        //}
    }
}
