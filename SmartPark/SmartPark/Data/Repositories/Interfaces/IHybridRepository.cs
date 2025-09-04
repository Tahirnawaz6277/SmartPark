using SmartPark.Models;

namespace SmartPark.Data.Repositories.Interfaces
{
    public interface IHybridRepository
    {
        Task<Role?> GetDriverRoleAsync();
        Task<User?> GetUserByEmailAsync(string email);
        Task<DateTime> GetDbServerTime();


    }
}
