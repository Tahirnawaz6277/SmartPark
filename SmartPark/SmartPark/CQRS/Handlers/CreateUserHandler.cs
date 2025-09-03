using MediatR;
using SmartPark.CQRS.Commands;
using SmartPark.Dtos;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand,UserResponseDto>
    {
        private readonly IUserService _userService;
        public CreateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.CreateUserAsync(request.RequestDto);
        }
    }
}
