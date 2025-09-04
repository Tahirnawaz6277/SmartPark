using SmartPark.Data.Repositories.Interfaces;
using SmartPark.Dtos;
using SmartPark.Models;
using SmartPark.Services.Interfaces;

namespace SmartPark.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICryptoService _cryptoService;
        public UserService(IUnitOfWork unitOfWork, ICryptoService cryptoService)
        {
            _unitOfWork = unitOfWork;
            _cryptoService = cryptoService;
        }

        public async Task<UserResponseDto?> CreateUserAsync(UserRequestDto requestDto)
        {
            var exists = await _unitOfWork.Repository<User>()
              .AnyAsync(u => u.Email == requestDto.Email || u.PhoneNumber == requestDto.PhoneNumber);

            if (exists)
            {
                throw new Exception("Email or phone already registered");
            }
            var user = new User
            {
                Name = requestDto.Name,
                Email = requestDto.Email,
                City = requestDto.City,
                PhoneNumber = requestDto.PhoneNumber,
            };
            var role = await _unitOfWork.HybridRepository.GetDriverRoleAsync();
            if (role == null)
            {
                throw new Exception("role not found");
                // will handle it globle exception handling later
            }
            user.Password = _cryptoService.Encrypt(requestDto.Password);    
            user.RoleId = role.Id;
            var newEntry =  await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            var responseDto = new UserResponseDto
            {
                Id = newEntry.Id,
                Name = newEntry.Name,
                Email = newEntry.Email,
                PhoneNumber = newEntry.PhoneNumber,
            };

            return responseDto;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<User>().GetByIdAsync(id);
        }
    }
}
