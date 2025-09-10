using SmartPark.Data.Contexts;
using SmartPark.Dtos.Location;
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

        public async Task<LocationReponse> CreateLocationAsync(LocationRequest dto, CancellationToken cancellationToken)
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
                    TotalSlots = dto.TotalSlots,
                    UserId = userId ?? Guid.Empty,
                    CreatedAt = serverTime,
                    CreatedBy = userId,
                };

                _dbContext.ParkingLocations.Add(location);
                await _dbContext.SaveChangesAsync(cancellationToken);
                // handl slots insertion based on total slots in location

                for (int i = 1; i < location.TotalSlots; i++)
                {
                    location.Slots.Add(new Slot
                    {
                        LocationId = location.Id,
                        SlotNumber = $"S{i}",
                        IsAvailable = true
                    });
                }
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                // Map to response with calculated total
                var result = MapToResponse(location);
                
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw; 
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

        //public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        //{
        //    return await _dbContext.ParkingLocations
        //        .Include(l => l.LocationSlots)
        //        .ThenInclude(ls => ls.Slot)
        //        .Select(l => new LocationDto
        //        {
        //            Id = l.Id,
        //            Name = l.Name,
        //            Address = l.Address,
        //            City = l.City,
        //            Image = l.Image,
        //            UserId = l.UserId,
        //            TimeStamp = l.TimeStamp,
        //            TotalSlots = l.LocationSlots.Sum(ls => ls.SlotCount),
        //            SmallSlots = l.LocationSlots
        //                .Where(ls => ls.Slot.SlotType == SmallType)
        //                .Sum(ls => ls.SlotCount),
        //            LargeSlots = l.LocationSlots
        //                 .Where(ls => ls.Slot.SlotType == LargeType)
        //                 .Sum(ls => ls.SlotCount),
        //            MediumSlots = l.LocationSlots
        //                 .Where(ls => ls.Slot.SlotType == MediumType)
        //                 .Sum(ls => ls.SlotCount),
        //            Slots = l.LocationSlots.Select(ls => new SlotSummaryDto
        //            {
        //                SlotType = ls.Slot.SlotType,
        //                SlotCount = ls.SlotCount,
        //                IsAvailable = ls.Slot.IsAvailable
        //            }).ToList()
        //        })
        //        .ToListAsync();
        //}

        //public async Task<LocationDto?> GetLocationByIdAsync(Guid id)
        //{
        //    return await _dbContext.ParkingLocations
        //        .Include(l => l.LocationSlots)
        //        .ThenInclude(ls => ls.Slot)
        //        .Where(l => l.Id == id)
        //        .Select(l => new LocationDto
        //        {
        //            Id = l.Id,
        //            Name = l.Name,
        //            Address = l.Address,
        //            City = l.City,
        //            Image = l.Image,
        //            UserId = l.UserId,
        //            TimeStamp = l.TimeStamp,
        //            TotalSlots = l.LocationSlots.Sum(ls => ls.SlotCount),
        //            SmallSlots = l.LocationSlots
        //                .Where(ls => ls.Slot.SlotType == SmallType)
        //                .Sum(ls => ls.SlotCount),
        //            LargeSlots = l.LocationSlots
        //                 .Where(ls => ls.Slot.SlotType == LargeType)
        //                 .Sum(ls => ls.SlotCount),
        //            MediumSlots = l.LocationSlots
        //                 .Where(ls => ls.Slot.SlotType == MediumType)
        //                 .Sum(ls => ls.SlotCount),
        //            Slots = l.LocationSlots.Select(ls => new SlotSummaryDto
        //            {
        //                SlotType = ls.Slot.SlotType,
        //                SlotCount = ls.SlotCount,
        //                IsAvailable = ls.Slot.IsAvailable
        //            }).ToList()
        //        })
        //        .FirstOrDefaultAsync();
        //}


        //public async Task<CreateLocationReponse> UpdateLocationAsync(Guid id, CreateLocationRequest dto)
        //{
        //    var location = await _dbContext.ParkingLocations
        //        .Include(l => l.LocationSlots)
        //        .FirstOrDefaultAsync(l => l.Id == id);

        //    if (location == null)
        //        throw new NotFoundException("Location not found");

        //    location.Name = dto.Name ?? location.Name;
        //    location.Address = dto.Address ?? location.Address;
        //    location.City = dto.City ?? location.City;
        //    location.Image = dto.Image ?? location.Image;

        //    // Update slot counts if provided
        //    var smallSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == SmallType, cancellationToken: default);
        //    var largeSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == LargeType, cancellationToken: default);
        //    var mediumSlot = await _dbContext.Slots.FirstAsync(s => s.SlotType == MediumType, cancellationToken: default);
        //    var smallLocationSlot = location.LocationSlots.FirstOrDefault(ls => ls.SlotId == smallSlot.Id);
        //    var largeLocationSlot = location.LocationSlots.FirstOrDefault(ls => ls.SlotId == largeSlot.Id);
        //    var mediumLocationSlot = location.LocationSlots.FirstOrDefault(ls => ls.SlotId == mediumSlot.Id);

        //    if (smallLocationSlot != null) smallLocationSlot.SlotCount = dto.SmallSlotCount;
        //    if (largeLocationSlot != null) largeLocationSlot.SlotCount = dto.LargeSlotCount;
        //    if (mediumLocationSlot != null) mediumLocationSlot.SlotCount = dto.MediumSlotCount;

        //    await _dbContext.SaveChangesAsync();

        //    return MapToResponse(location);
        //}

        private LocationReponse MapToResponse(ParkingLocation loc)
        {
            return new LocationReponse
            {
                Id = loc.Id,
                Name = loc.Name ?? "",
                Address = loc.Address ?? "",
                City = loc.City,
                TotalSlots = loc.TotalSlots
            };
        }
    }
}