using SmartPark.Dtos;
using SmartPark.Models;

namespace SmartPark.Services.Interfaces
{
    public interface IUserService
    {
        Task<Guid> CreateUserAsync(UserRequestDto requestDto);
        Task<User?> GetUserByIdAsync(Guid id);
    }
}
