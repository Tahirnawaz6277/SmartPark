using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Data.Repositories.Interfaces;
using SmartPark.Models;

namespace SmartPark.Data.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ParkingDbContext _dbContext;
        public RoleRepository(ParkingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Role?> GetDriverRoleAsync()
        {
            string driverRole = "Driver".ToLower();
            return await _dbContext.Roles
                .Where(x => driverRole.Contains(x.RoleName.ToLower()))
                .FirstOrDefaultAsync();
        }
    }
}
