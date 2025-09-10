using Microsoft.EntityFrameworkCore;
using SmartPark.Common.Enum;
using SmartPark.Common.Helpers;
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

        public async Task<CreateLocationReponse> CreateLocationAsync(CreateLocationRequest dto, CancellationToken cancellationToken)
        {
            string smalltype = NanoHelpers.GetEnumDescription(SlotType.small);
            string largetype = NanoHelpers.GetEnumDescription(SlotType.large);
            string medium = NanoHelpers.GetEnumDescription(SlotType.medium);
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
                    TotalSlots = (dto.SmallSlotCount + dto.LargeSlotCount),
                    UserId = userId ?? Guid.Empty,
                    TimeStamp = serverTime
                };

                _dbContext.ParkingLocations.Add(location);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Link to existing slot types with counts
                var smallSlot = await _dbContext.Slots
                    .FirstAsync(s => s.SlotType == smalltype, cancellationToken);
                var largeSlot = await _dbContext.Slots
                    .FirstAsync(s => s.SlotType == largetype, cancellationToken);      
                var mediumSlot = await _dbContext.Slots
                    .FirstAsync(s => s.SlotType == medium, cancellationToken);

                var locationSlots = new List<LocationSlot>
                {
                    new LocationSlot { LocationId = location.Id, SlotId = smallSlot.Id, SlotCount = dto.SmallSlotCount },
                    new LocationSlot { LocationId = location.Id, SlotId = largeSlot.Id, SlotCount = dto.LargeSlotCount },
                    new LocationSlot { LocationId = location.Id, SlotId = largeSlot.Id, SlotCount = dto.MediumSlotCount }
                };

                _dbContext.LocationSlots.AddRange(locationSlots);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                // Map to response with calculated total
                var result = MapToResponse(location);
                result.TotalSlots = locationSlots.Sum(ls => ls.SlotCount);
                //result.Slots.Add(new SlotSummaryDto { SlotType = "small", SlotCount = dto.SmallSlotCount, IsAvailable = smallSlot.IsAvailable });
                //result.Slots.Add(new SlotSummaryDto { SlotType = "large", SlotCount = dto.LargeSlotCount, IsAvailable = largeSlot.IsAvailable });

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

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            string smalltype = NanoHelpers.GetEnumDescription(SlotType.small);
            string largetype = NanoHelpers.GetEnumDescription(SlotType.large);
            string medium = NanoHelpers.GetEnumDescription(SlotType.medium);

            return await _dbContext.ParkingLocations
                .Include(l => l.LocationSlots)
                .ThenInclude(ls => ls.Slot)
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    City = l.City,
                    Image = l.Image,
                    UserId = l.UserId,
                    TimeStamp = l.TimeStamp,
                    TotalSlots = l.LocationSlots.Sum(ls => ls.SlotCount), // Calculate from junction table
                    SmallSlots = l.LocationSlots
                        .Where(ls => ls.Slot.SlotType == smalltype)
                        .Sum(ls => ls.SlotCount),
                    LargeSlots = l.LocationSlots
                         .Where(ls => ls.Slot.SlotType == largetype)
                         .Sum(ls => ls.SlotCount),  
                    MediumSlots = l.LocationSlots
                         .Where(ls => ls.Slot.SlotType == medium)
                         .Sum(ls => ls.SlotCount),
                    Slots = l.LocationSlots.Select(ls => new SlotSummaryDto
                    {
                        SlotType = ls.Slot.SlotType,
                        SlotCount = ls.SlotCount,
                        IsAvailable = ls.Slot.IsAvailable
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<LocationDto?> GetLocationByIdAsync(Guid id)
        {
            string smalltype = NanoHelpers.GetEnumDescription(SlotType.small);
            string largetype = NanoHelpers.GetEnumDescription(SlotType.large);
            string medium = NanoHelpers.GetEnumDescription(SlotType.medium);

            return await _dbContext.ParkingLocations
                .Include(l => l.LocationSlots)
                .ThenInclude(ls => ls.Slot)
                .Where(l => l.Id == id)
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    City = l.City,
                    Image = l.Image,
                    UserId = l.UserId,
                    TimeStamp = l.TimeStamp,
                    TotalSlots = l.LocationSlots.Sum(ls => ls.SlotCount), // Calculate from junction table
                    SmallSlots = l.LocationSlots
                        .Where(ls => ls.Slot.SlotType == smalltype && l.Id == id)
                        .Sum(ls => ls.SlotCount),
                    LargeSlots = l.LocationSlots
                         .Where(ls => ls.Slot.SlotType == largetype && l.Id == id)
                         .Sum(ls => ls.SlotCount),
                    MediumSlots = l.LocationSlots
                         .Where(ls => ls.Slot.SlotType == medium && l.Id == id)
                         .Sum(ls => ls.SlotCount),
                    Slots = l.LocationSlots.Select(ls => new SlotSummaryDto
                    {
                        SlotType = ls.Slot.SlotType,
                        SlotCount = ls.SlotCount,
                        IsAvailable = ls.Slot.IsAvailable
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CreateLocationReponse> UpdateLocationAsync(Guid id, CreateLocationRequest dto)
        {
            string smalltype = NanoHelpers.GetEnumDescription(SlotType.small);
            string largetype = NanoHelpers.GetEnumDescription(SlotType.large);
            string medium = NanoHelpers.GetEnumDescription(SlotType.medium);

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
            var smallSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == smalltype, cancellationToken: default);
            var largeSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == largetype, cancellationToken: default);
            var mediumSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == medium, cancellationToken: default);
            var smallLocationSlot = location.LocationSlots.FirstOrDefault(ls => ls.SlotId == smallSlot.Id);
            var largeLocationSlot = location.LocationSlots.FirstOrDefault(ls => ls.SlotId == largeSlot.Id);
            var mediumLocationSlot = location.LocationSlots.FirstOrDefault(ls => ls.SlotId == mediumSlot.Id);

            if (smallLocationSlot != null) smallLocationSlot.SlotCount = dto.SmallSlotCount;
            if (largeLocationSlot != null) largeLocationSlot.SlotCount = dto.LargeSlotCount;
            if (mediumLocationSlot != null) mediumLocationSlot.SlotCount = dto.MediumSlotCount;

            await _dbContext.SaveChangesAsync();

            return MapToResponse(location);
        }

        private CreateLocationReponse MapToResponse(ParkingLocation loc)
        {
            return new CreateLocationReponse
            {
                LocationId = loc.Id,
                Name = loc.Name ?? "",
                Address = loc.Address ?? "",
                City = loc.City,
                TotalSlots = loc.LocationSlots?.Sum(ls => ls.SlotCount) ?? 0,
            };
        }
    }
}