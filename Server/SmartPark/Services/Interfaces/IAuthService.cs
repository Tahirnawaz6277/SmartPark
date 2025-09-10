using SmartPark.Dtos.UserDtos;

namespace SmartPark.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> AuthenticateAsync(string email, string password);

    }
}
