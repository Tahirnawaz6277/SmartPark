using MediatR;
using SmartPark.CQRS.Queries.User;
using SmartPark.Dtos.UserDtos;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.User
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto?>
    {
        private readonly IUserService _userService;
        public GetUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserDto?> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
           return await _userService.GetUserByIdAsync(request.Id);
        }
    }
}
