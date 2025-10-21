using MediatR;
using SmartPark.CQRS.Queries.User;
using SmartPark.Dtos.UserDtos;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.User
{
    public class ProfileHandler : IRequestHandler<ProfileQuery, ProfileDto>
    {
        private readonly IUserService _userService;
        public ProfileHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<ProfileDto?> Handle(ProfileQuery request, CancellationToken cancellationToken)
        {
            return (ProfileDto?)await _userService.GetUserProfile();
        }
    }
}
