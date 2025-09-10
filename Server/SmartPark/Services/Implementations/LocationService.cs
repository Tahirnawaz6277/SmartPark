//using Microsoft.EntityFrameworkCore;
//using SmartPark.Data.Contexts;
//using SmartPark.Dtos.Location;
//using SmartPark.Dtos.Slot;
//using SmartPark.Models;
//using SmartPark.Services.Interfaces;

//namespace SmartPark.Services.Implementations
//{
//    public class LocationService : ILocationService
//    {
//        private readonly ParkingDbContext _dbContext;
//        private readonly IHelper _helper;

//        public LocationService(ParkingDbContext dbContext, IHelper helper)
//        {
//            _dbContext = dbContext;
//            _helper = helper;
//        }

//        public async Task<LocationResponseDto> CreateLocationAsync(LocationRequestDto dto, CancellationToken cancellationToken)
//        {
//            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
//            var userId = await _helper.GetUserIdFromToken();
//            var serverTime = await _helper.GetDatabaseTime();
//            try
//            {
//                // Create the location
//                var location = new ParkingLocation
//                {
//                    Name = dto.Name,
//                    Address = dto.Address,
//                    City = dto.City,
//                    Image = dto.Image,
//                    TotalSlots = (dto.SmallSlotCount + dto.LargeSlotCount) ,
//                    UserId = userId, // logged in user
//                    TimeStamp = serverTime
//                };


//                _dbContext.ParkingLocations.Add(location);
//                await _dbContext.SaveChangesAsync();
//                // Link to existing slot types with counts
//                var smallSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == "small", cancellationToken);
//                var largeSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == "large", cancellationToken);

//                var locationSlots = new List<LocationSlot>
//                {
//                    new LocationSlot { LocationId = location.Id, SlotId = smallSlot.Id, SlotCount = dto.SmallSlotCount },
//                    new LocationSlot { LocationId = location.Id, SlotId = largeSlot.Id, SlotCount = dto.LargeSlotCount }
//                };

//                _dbContext.LocationSlots.AddRange(locationSlots);
//                await _dbContext.SaveChangesAsync(cancellationToken);
//                await transaction.CommitAsync(cancellationToken);
//                var result = MapToResponse(location);
//                result.Slots.Add(new SlotSummaryDto { SlotType = "small", SlotCount = dto.SmallSlotCount, IsAvailable = smallSlot.IsAvailable });
//                result.Slots.Add(new SlotSummaryDto { SlotType = "large", SlotCount = dto.LargeSlotCount, IsAvailable = largeSlot.IsAvailable });

//                return result;
//            }
//            catch (Exception)
//            {
//                await transaction.RollbackAsync(cancellationToken);
//                throw; // Let the mediator handle the exception or return a custom error
//            }
//        }

//        public async Task<bool> DeleteLocationAsync(Guid id)
//        {
//            var location = await _dbContext.ParkingLocations.FindAsync(id);
//            if (location == null) return false;

//            _dbContext.ParkingLocations.Remove(location);
//            await _dbContext.SaveChangesAsync();

//            return true;
//        }

//        public async Task<IEnumerable<LocationResponseDto>> GetAllLocationsAsync()
//        {
//            return await _dbContext.ParkingLocations
//        .Select(l => new LocationResponseDto
//        {
//            Id = l.Id,
//            Name = l.Name,
//            Address = l.Address,
//            TotalSlots = l.TotalSlots,
//            City = l.City,
//            Image = l.Image,
//            UserId = l.UserId,
//            Slots = l.Slots.Select(s => new SlotSummaryDto
//            {
//                //Id = s.Id,
//                SlotType = s.SlotType,
//                IsAvailable = (bool)s.IsAvailable
//            }).ToList()
//        })
//        .ToListAsync();
//        }

//        public async Task<LocationResponseDto?> GetLocationByIdAsync(Guid id)
//        {
//            var location = await _dbContext.ParkingLocations
//            .Where(l => l.Id == id)
//            .Select(l => new LocationResponseDto
//            {
//                Id = l.Id,
//                Name = l.Name,
//                Address = l.Address,
//                TotalSlots = l.TotalSlots,
//                City = l.City,
//                Image = l.Image,
//                UserId = l.UserId,
//                Slots = l.Slots.Select(s => new SlotSummaryDto
//                {
//                    //Id = s.Id,
//                    SlotType = s.SlotType,
//                    IsAvailable = (bool)s.IsAvailable,
//                }).ToList()
//            })
//            .FirstOrDefaultAsync();

//            return location;
//        }

//        public async Task<LocationResponseDto> UpdateLocationAsync(Guid id, LocationRequestDto dto)
//        {
//            var location = await _dbContext.ParkingLocations.FindAsync(id);
//            if (location == null)
//                throw new KeyNotFoundException("Location not found");

//            location.Name = dto.Name ?? location.Name;
//            location.Address = dto.Address ?? location.Address;
//            //location.TotalSlots = dto.TotalSlots ?? location.TotalSlots;
//            location.City = dto.City ?? location.City;
//            location.Image = dto.Image ?? location.Image;
//            //location.UserId = dto.UserId ?? location.UserId;

//            await _dbContext.SaveChangesAsync();

//            return MapToResponse(location);
//        }

//        private static LocationResponseDto MapToResponse(ParkingLocation loc) =>
//           new LocationResponseDto
//           {
//               Id = loc.Id,
//               Name = loc.Name,
//               Address = loc.Address,
//               TotalSlots = loc.TotalSlots,
//               City = loc.City,
//               Image = loc.Image,
//               UserId = loc.UserId,
//               //Slots = loc.Slots?.Select(s => new SlotSummaryDto
//               //{
//               //    Id = s.Id,
//               //    SlotType = s.SlotType,
//               //    IsAvailable = s.IsAvailable
//               //}).ToList() ?? new List<SlotResponseDto>()
//           };

//    }
//}


using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Dtos.Location;
using SmartPark.Dtos.Slot;
using SmartPark.Exceptions;
using SmartPark.Models;
using SmartPark.Services.Interfaces;

namespace SmartPark.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly ParkingDbContext _dbContext;
        private readonly IHelper _helper;

        public LocationService(ParkingDbContext dbContext, IHelper helper)
        {
            _dbContext = dbContext;
            _helper = helper;
        }

        public async Task<LocationResponseDto> CreateLocationAsync(LocationRequestDto dto, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Get user ID and server time
                var userId = await _helper.GetUserIdFromToken();
                var serverTime = await _helper.GetDatabaseTime();

                // Create the location with Guid
                var location = new ParkingLocation
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    City = dto.City,
                    Image = dto.Image,
                    TotalSlots = (dto.SmallSlotCount+dto.LargeSlotCount),
                    UserId = userId ?? Guid.Empty,
                    TimeStamp = serverTime
                };

                _dbContext.ParkingLocations.Add(location);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Link to existing slot types with counts
                var smallSlot = await _dbContext.Slots
                    .FirstAsync(s => s.SlotType == "small", cancellationToken);
                var largeSlot = await _dbContext.Slots
                    .FirstAsync(s => s.SlotType == "large", cancellationToken);

                var locationSlots = new List<LocationSlot>
                {
                    new LocationSlot { LocationId = location.Id, SlotId = smallSlot.Id, SlotCount = dto.SmallSlotCount },
                    new LocationSlot { LocationId = location.Id, SlotId = largeSlot.Id, SlotCount = dto.LargeSlotCount }
                };

                _dbContext.LocationSlots.AddRange(locationSlots);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                // Map to response with calculated total
                var result = MapToResponse(location);
                result.TotalSlots = locationSlots.Sum(ls => ls.SlotCount);
                result.Slots.Add(new SlotSummaryDto { SlotType = "small", SlotCount = dto.SmallSlotCount, IsAvailable = smallSlot.IsAvailable });
                result.Slots.Add(new SlotSummaryDto { SlotType = "large", SlotCount = dto.LargeSlotCount, IsAvailable = largeSlot.IsAvailable });

                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw; // Let the mediator handle the exception or return a custom error
            }
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
                .Include(l => l.LocationSlots)
                .ThenInclude(ls => ls.Slot)
                .Select(l => new LocationResponseDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    City = l.City,
                    Image = l.Image,
                    UserId = l.UserId,
                    TimeStamp = l.TimeStamp,
                    TotalSlots = l.LocationSlots.Sum(ls => ls.SlotCount), // Calculate from junction table
                    Slots = l.LocationSlots.Select(ls => new SlotSummaryDto
                    {
                        SlotType = ls.Slot.SlotType,
                        SlotCount = ls.SlotCount,
                        IsAvailable = ls.Slot.IsAvailable
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<LocationResponseDto?> GetLocationByIdAsync(Guid id)
        {
            return await _dbContext.ParkingLocations
                .Include(l => l.LocationSlots)
                .ThenInclude(ls => ls.Slot)
                .Where(l => l.Id == id)
                .Select(l => new LocationResponseDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    City = l.City,
                    Image = l.Image,
                    UserId = l.UserId,
                    TimeStamp = l.TimeStamp,
                    TotalSlots = l.LocationSlots.Sum(ls => ls.SlotCount), // Calculate from junction table
                    Slots = l.LocationSlots.Select(ls => new SlotSummaryDto
                    {
                        SlotType = ls.Slot.SlotType,
                        SlotCount = ls.SlotCount,
                        IsAvailable = ls.Slot.IsAvailable
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<LocationResponseDto> UpdateLocationAsync(Guid id, LocationRequestDto dto)
        {
            var location = await _dbContext.ParkingLocations
                .Include(l => l.LocationSlots)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
                throw new NotFoundException("Location not found");

            location.Name = dto.Name ?? location.Name;
            location.Address = dto.Address ?? location.Address;
            location.City = dto.City ?? location.City;
            location.Image = dto.Image ?? location.Image;

            // Update slot counts if provided
            var smallSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == "small", cancellationToken: default);
            var largeSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == "large", cancellationToken: default);
            var smallLocationSlot = location.LocationSlots.FirstOrDefault(ls => ls.SlotId == smallSlot.Id);
            var largeLocationSlot = location.LocationSlots.FirstOrDefault(ls => ls.SlotId == largeSlot.Id);

            if (smallLocationSlot != null) smallLocationSlot.SlotCount = dto.SmallSlotCount;
            if (largeLocationSlot != null) largeLocationSlot.SlotCount = dto.LargeSlotCount;

            await _dbContext.SaveChangesAsync();

            return MapToResponse(location);
        }

        private LocationResponseDto MapToResponse(ParkingLocation loc)
        {
            return new LocationResponseDto
            {
                Id = loc.Id,
                Name = loc.Name,
                Address = loc.Address,
                City = loc.City,
                Image = loc.Image,
                UserId = loc.UserId,
                TimeStamp = loc.TimeStamp,
                TotalSlots = loc.LocationSlots?.Sum(ls => ls.SlotCount) ?? 0,
                Slots = loc.LocationSlots?.Select(ls => new SlotSummaryDto
                {
                    SlotType = ls.Slot.SlotType,
                    SlotCount = ls.SlotCount,
                    IsAvailable = ls.Slot.IsAvailable
                }).ToList() ?? new List<SlotSummaryDto>()
            };
        }
    }
}