using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Data.Repositories.Interfaces;
using SmartPark.Models;

namespace SmartPark.Data.Repositories.Implementations
{
    public class HybridRepository : IHybridRepository
    {
        private readonly ParkingDbContext _dbContext;
        public HybridRepository(ParkingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DateTime> GetDbServerTime()
        {
            return await _dbContext.Database.SqlQueryRaw<DateTime>("SELECT GETDATE()").FirstOrDefaultAsync();
        }

        public async Task<Role?> GetDriverRoleAsync()
        {
            string driverRole = "Driver".ToLower();
            return await _dbContext.Roles
                .Where(x => driverRole.Contains(x.RoleName.ToLower()))
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users
                           .FirstOrDefaultAsync(un => un.Email == email);
        }
       

    }
}
