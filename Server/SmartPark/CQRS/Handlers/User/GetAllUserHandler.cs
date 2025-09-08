using MediatR;
using SmartPark.CQRS.Queries.User;
using SmartPark.Dtos.UserDtos;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.User
{
    public class GetAllUserHandler : IRequestHandler<GetAllUserQuery, List<UserDto>>
    {
        private readonly IUserService _userService;
        public GetAllUserHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<List<UserDto?>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return (List<UserDto?>)await _userService.GetAllUserAsync();
        }
    }
}
