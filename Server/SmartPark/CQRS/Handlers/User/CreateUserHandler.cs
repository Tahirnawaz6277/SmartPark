using MediatR;
using SmartPark.CQRS.Commands.User;
using SmartPark.Dtos.UserDtos;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.User
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand,RegistrationResponse>
    {
        private readonly IUserService _userService;
        public CreateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<RegistrationResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.CreateUserAsync(request.RequestDto);
        }
    }
}
