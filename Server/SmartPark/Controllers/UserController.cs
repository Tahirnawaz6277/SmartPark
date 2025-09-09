using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPark.Common.Wrapper;
using SmartPark.CQRS.Commands;
using SmartPark.CQRS.Commands.User;
using SmartPark.CQRS.Queries.User;
using SmartPark.Dtos.UserDtos;

namespace SmartPark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestDto)
        {
            var query = new LoginQuery(requestDto.Email,requestDto.Password);
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
            var query = new GetUserQuery(id);
            var user = await _mediator.Send(query);
            return user != null ? Ok(user) : NotFound();
        }


        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            var query = new GetAllUserQuery();
            var users = await _mediator.Send(query);
            return Ok(users);
        }


        [HttpPut("update-user/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest updateUser)
        {
            var command = new UpdateUserCommand(id, updateUser);
            var updatedUser = await _mediator.Send(command);

            return Ok(new
            {
                Message = "User updated successfully",
                Data = updatedUser
            });
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var command = new DeleteUserCommad(id);
            var deletedUserId = await _mediator.Send(command);

            return Ok(new
            {
                Message = "User deleted successfully",
                Data = deletedUserId
            });
        }




    }
}
