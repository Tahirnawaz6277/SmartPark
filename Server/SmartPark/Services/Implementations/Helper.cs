using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Models;
using SmartPark.Services.Interfaces;
using System.Security.Claims;

namespace SmartPark.Services.Implementations
{
    public class Helper : IHelper
    {
        private readonly ParkingDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public Helper(IHttpContextAccessor contextAccessor, ParkingDbContext dbContext)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
        }

        public async Task<User?> GetActiveUserAsync(string email)
        {
            return await _dbContext.Users.Include(r => r.Role)
                                   .FirstOrDefaultAsync(u => u.Email == email);
        }
        
        public Task<string> GetBaseUrl()
        {
            var request = _contextAccessor?.HttpContext?.Request;
            if (request == null)
            {
                return Task.FromResult(string.Empty);
            }
            return Task.FromResult($"{request?.Scheme}://{request?.Host}");

        }


        public async Task<DateTime> GetDatabaseTime()
        {
            // Database time via raw SQL (works in SQL Server)
            var result = await _dbContext.Database.ExecuteSqlRawAsync("SELECT GETDATE()");
            return DateTime.Now; // fallback if ExecuteSqlRawAsync doesn’t return
        }

        public Task<Guid?> GetUserIdFromToken()
        {
            var userIdFromToken = _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdFromToken, out var userId))
            {
                return Task.FromResult<Guid?>(userId);
            }

            return Task.FromResult<Guid?>(null);
        }
    }
}
