using SmartPark.Dtos.UserDtos;

namespace SmartPark.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserLoginResponse> AuthenticateAsync(string email, string password);

    }
}
