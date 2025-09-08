using SmartPark.Dtos.Location;

namespace SmartPark.Services.Interfaces
{
    public interface ILocationService
    {
        Task<LocationResponseDto> CreateLocationAsync(LocationRequestDto dto);
        Task<LocationResponseDto?> GetLocationByIdAsync(Guid id);
        Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync();
        Task<LocationResponseDto> UpdateLocationAsync(Guid id, LocationRequestDto dto);
        Task<bool> DeleteLocationAsync(Guid id);
    }
}
