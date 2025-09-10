using SmartPark.Dtos.Location;

namespace SmartPark.Services.Interfaces
{
    public interface ILocationService
    {
        Task<LocationReponse> CreateLocationAsync(LocationRequest dto, CancellationToken cancellationToken);
        Task<LocationDto?> GetLocationByIdAsync(Guid id);
        Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        Task<LocationReponse> UpdateLocationAsync(Guid id, LocationRequest dto);
        Task<bool> DeleteLocationAsync(Guid id);
    }
}
