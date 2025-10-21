using Microsoft.EntityFrameworkCore; // use EF Core
using SmartPark.Data.Contexts;
using SmartPark.Dtos.UserDtos;
using SmartPark.Exceptions;
using SmartPark.Models;
using SmartPark.Services.Interfaces;

namespace SmartPark.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ICryptoService _cryptoService;
        private readonly ParkingDbContext _dbContext;
        private readonly IHelper _helper;

        public UserService(ICryptoService cryptoService, ParkingDbContext dbContext, IHelper helper)
        {
            _cryptoService = cryptoService;
            _dbContext = dbContext;
            _helper = helper;
        }

        public async Task<RegistrationResponse?> CreateUserAsync(RegistrationRequest requestDto)
        {
            // check duplicates
            var exists = await _dbContext.Users.AnyAsync(u =>
                u.Email == requestDto.Email || u.PhoneNumber == requestDto.PhoneNumber);

            if (exists)
                throw new ConflictException("Email or phone already registered");

            // fetch role
            var role = await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.RoleName.ToLower() == "driver");

            if (role == null)
                throw new NotFoundException("Role 'Driver' not found");

            // create new user
            var user = new User
            {
                Name = requestDto.Name,
                Email = requestDto.Email,
                City = requestDto.City,
                PhoneNumber = requestDto.PhoneNumber,
                Password = _cryptoService.Encrypt(requestDto.Password),
                RoleId = role.Id
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return new RegistrationResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var domainUser = await _dbContext.Users
                .Include(r => r.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            return domainUser == null ? null : new UserDto
            {
                Id = domainUser.Id,
                Name = domainUser.Name,
                Email = domainUser.Email,
                PhoneNumber = domainUser.PhoneNumber,
                City = domainUser.City,
                RoleId = domainUser.RoleId,
                RoleName = domainUser.Role.RoleName
            };
        }

        public async Task<IEnumerable<UserDto?>> GetAllUserAsync()
        {
            return await _dbContext.Users
                    .Include(u => u.Role) // include related role
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        Address = u.Address,
                        PhoneNumber = u.PhoneNumber,
                        City = u.City,
                        RoleId = u.RoleId,
                        RoleName = u.Role.RoleName
                    })
                    .ToListAsync();
        }

        public async Task<RegistrationResponse> UpdateUserAsync(Guid id, UpdateUserRequest requestDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id/* && !u.IsDeleted*/);

            if (user == null)
                throw new NotFoundException("User not found");

            // Update fields
            user.Name = requestDto.Name;
            user.Email = requestDto.Email;
            user.PhoneNumber = requestDto.PhoneNumber;
            user.Address = requestDto.Address;
            user.City = requestDto.City;

            await _dbContext.SaveChangesAsync();

            return new RegistrationResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) throw new NotFoundException("User not found");

            user.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ProfileDto?> GetUserProfile()
        {
            var loggedInUserId = await _helper.GetUserIdFromToken();

            return await _dbContext.Users
                .Where(x => x.Id == loggedInUserId)
                .Select(u => new ProfileDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    City = u.City,
                    //picture = u.Picture
                }).FirstOrDefaultAsync();

        }
    }
}
