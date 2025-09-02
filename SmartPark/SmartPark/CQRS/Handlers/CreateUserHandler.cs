using MediatR;
using SmartPark.CQRS.Commands;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand,Guid>
    {
        private readonly IUserService _userService;
        public CreateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _userService.CreateUserAsync(request.RequestDto);
        }
    }
}
