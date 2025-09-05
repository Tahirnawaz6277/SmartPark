using MediatR;
using SmartPark.CQRS.Commands;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.User
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommad, Guid>
    {
        private readonly IUserService _userService;
        public DeleteUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Guid> Handle(DeleteUserCommad request, CancellationToken cancellationToken)
        {
            await _userService.DeleteUserAsync(request.Id);
            return request.Id;
        }
    }
}
