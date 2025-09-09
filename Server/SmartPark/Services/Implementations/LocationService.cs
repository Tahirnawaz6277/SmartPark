using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Dtos.Location;
using SmartPark.Dtos.Slot;
using SmartPark.Models;
using SmartPark.Services.Interfaces;

namespace SmartPark.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly ParkingDbContext _dbContext;

        public LocationService(ParkingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<LocationResponseDto> CreateLocationAsync(LocationRequestDto dto)
        {
            var location = new ParkingLocation
            {
                Name = dto.Name,
                Address = dto.Address,
                TotalSlots = dto.TotalSlots,
                City = dto.City,
                Image = dto.Image,
                UserId = dto.UserId,
                TimeStamp = DateTime.UtcNow
            };

            _dbContext.ParkingLocations.Add(location);
            await _dbContext.SaveChangesAsync();
            return MapToResponse(location);
        }

        public async Task<bool> DeleteLocationAsync(Guid id)
        {
            var location = await _dbContext.ParkingLocations.FindAsync(id);
            if (location == null) return false;

            _dbContext.ParkingLocations.Remove(location);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync()
        {
            return await _dbContext.ParkingLocations
        .Select(l => new LocationResponseDto
        {
            Id = l.Id,
            Name = l.Name,
            Address = l.Address,
            TotalSlots = l.TotalSlots,
            City = l.City,
            Image = l.Image,
            UserId = l.UserId,
            Slots = l.Slots.Select(s => new SlotResponseDto
            {
                Id = s.Id,
                SlotType = s.SlotType,
                IsAvailable = s.IsAvailable
            }).ToList()
        })
        .ToListAsync();
        }

        public async Task<LocationResponseDto?> GetLocationByIdAsync(Guid id)
        {
            var location = await _dbContext.ParkingLocations
            .Where(l => l.Id == id)
            .Select(l => new LocationResponseDto
            {
                Id = l.Id,
                Name = l.Name,
                Address = l.Address,
                TotalSlots = l.TotalSlots,
                City = l.City,
                Image = l.Image,
                UserId = l.UserId,
                Slots = l.Slots.Select(s => new SlotResponseDto
                {
                    Id = s.Id,
                    SlotType = s.SlotType,
                    IsAvailable = s.IsAvailable
                }).ToList()
            })
            .FirstOrDefaultAsync();

            return location;
        }

        public async Task<LocationResponseDto> UpdateLocationAsync(Guid id, LocationRequestDto dto)
        {
            var location = await _dbContext.ParkingLocations.FindAsync(id);
            if (location == null)
                throw new KeyNotFoundException("Location not found");

            location.Name = dto.Name ?? location.Name;
            location.Address = dto.Address ?? location.Address;
            location.TotalSlots = dto.TotalSlots ?? location.TotalSlots;
            location.City = dto.City ?? location.City;
            location.Image = dto.Image ?? location.Image;
            location.UserId = dto.UserId ?? location.UserId;

            await _dbContext.SaveChangesAsync();

            return MapToResponse(location);
        }

        private static LocationResponseDto MapToResponse(ParkingLocation loc) =>
           new LocationResponseDto
           {
               Id = loc.Id,
               Name = loc.Name,
               Address = loc.Address,
               TotalSlots = loc.TotalSlots,
               City = loc.City,
               Image = loc.Image,
               UserId = loc.UserId,
               Slots = loc.Slots?.Select(s => new SlotResponseDto
               {
                   Id = s.Id,
                   SlotType = s.SlotType,
                   IsAvailable = s.IsAvailable
               }).ToList() ?? new List<SlotResponseDto>()
           };

    }
}
