using SmartPark.Models;

namespace SmartPark.Services.Interfaces
{
    public interface IHelper
    {
        Task<User> GetActiveUserAsync(string userName);
        Task<int?> GetUserIdFromToken();
        Task<DateTime> GetDatabaseTime();
    }
}
