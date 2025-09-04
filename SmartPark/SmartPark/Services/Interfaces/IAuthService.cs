using SmartPark.Dtos;

namespace SmartPark.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserLoginResponse> AuthenticateAsync(string email, string password);

    }
}
