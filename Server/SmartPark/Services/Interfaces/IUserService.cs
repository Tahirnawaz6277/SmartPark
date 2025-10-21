using SmartPark.Dtos.UserDtos;

namespace SmartPark.Services.Interfaces
{
    public interface IUserService
    {
        Task<RegistrationResponse?> CreateUserAsync(RegistrationRequest requestDto);
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserDto?>> GetAllUserAsync();
        Task<ProfileDto?> GetUserProfile();
        Task<RegistrationResponse> UpdateUserAsync(Guid id, UpdateUserRequest requestDto);
        Task DeleteUserAsync(Guid id);
    }
}
