using SmartPark.Dtos.Location;

namespace SmartPark.Services.Interfaces
{
    public interface IBookingService
    {
        Task<LocationReponse> CreateBookingAsync(LocationRequest dto, CancellationToken cancellationToken);
        Task<LocationDto?> GetBookingByIdAsync(Guid id);
        Task<IEnumerable<LocationDto>> GetAllBookingsAsync();
        Task<LocationReponse> UpdateBookingAsync(Guid id, LocationRequest dto);
        Task<bool> DeleteBookingAsync(Guid id);
    }
}
