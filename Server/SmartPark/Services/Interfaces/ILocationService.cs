using SmartPark.Dtos.Location;

namespace SmartPark.Services.Interfaces
{
    public interface ILocationService
    {
        Task<LocationReponse> CreateLocationAsync(LocationRequest dto, CancellationToken cancellationToken);
        //Task<LocationDto?> GetLocationByIdAsync(Guid id);
        //Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
        //Task<CreateLocationReponse> UpdateLocationAsync(Guid id, CreateLocationRequest dto);
        Task<bool> DeleteLocationAsync(Guid id);
    }
}
