using SmartPark.Models;

namespace SmartPark.Data.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetDriverRoleAsync(); 
    }
}
