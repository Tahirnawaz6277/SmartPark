using MediatR;
using SmartPark.CQRS.Queries.User;
using SmartPark.Dtos.UserDtos;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.User
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, UserLoginResponse> 
    {
        private readonly IAuthService _authService;

        public LoginQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<UserLoginResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            return await _authService.AuthenticateAsync(request.Email,request.Password);
        }
    }
}
