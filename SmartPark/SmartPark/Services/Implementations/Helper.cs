using System.Security.Claims;
using SmartPark.Data.Repositories.Interfaces;
using SmartPark.Exceptions;
using SmartPark.Models;
using SmartPark.Services.Interfaces;

namespace SmartPark.Services.Implementations
{
    public class Helper : IHelper
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        public Helper(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<User> GetActiveUserAsync(string email)
        {
            var user = await _unitOfWork.HybridRepository.GetUserByEmailAsync(email);

            if (user is null)
            {
                throw new NotFoundException($"User with this '{email}' not found.");
            }
            return user;
        }


        public async Task<DateTime> GetDatabaseTime()
        {
            return await _unitOfWork.HybridRepository.GetDbServerTime();
        }

        public async Task<int?> GetUserIdFromToken()
        {

            var userIdFromToken = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdFromToken, out var userId))
            {
                return userId;
            }
            return null;
        }
    }
}
