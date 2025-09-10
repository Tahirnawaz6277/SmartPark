using SmartPark.Models;

namespace SmartPark.Services.Interfaces
{
    public interface IHelper
    {
        Task<User> GetActiveUserAsync(string userName);
        Task<Guid?> GetUserIdFromToken();
        Task<DateTime> GetDatabaseTime();
    }
}
