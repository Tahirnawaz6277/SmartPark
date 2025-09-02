using SmartPark.Data.Repositories.Interfaces;
using SmartPark.Dtos;
using SmartPark.Models;
using SmartPark.Services.Interfaces;

namespace SmartPark.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateUserAsync(UserRequestDto requestDto)
        {
            var user = new User
            {
                Name = requestDto.Name,
                Email = requestDto.Email,
                Password = requestDto.Password,
                City = requestDto.City
            };
            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return user.Id;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<User>().GetByIdAsync(id);
        }
    }
}
