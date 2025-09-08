using SmartPark.Dtos.UserDtos;

namespace SmartPark.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto?> CreateUserAsync(UserRequestDto requestDto);
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserDto?>> GetAllUserAsync();
        Task<UserResponseDto> UpdateUserAsync(Guid id, UpdateUserRequest requestDto);
        Task DeleteUserAsync(Guid id);
    }
}
