using System.Threading;
using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Dtos.Location;
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

        public async Task<LocationReponse> CreateLocationAsync(LocationRequest dto, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Get user ID and server time
                var userId = await _helper.GetUserIdFromToken();
                var serverTime = await _helper.GetDatabaseTime();
                string? imagePath = null;
                string? imageExtension = null;
                if (dto.ImageFile != null && dto.ImageFile.Length>0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    if (!allowedExtensions.Contains(imageExtension?.ToLower()))
                    {
                        throw new InvalidOperationException("Only .jpg, .jpeg, and .png files are allowed.");
                    }
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "locations");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var extension = Path.GetExtension(dto.ImageFile.FileName).ToLower();
                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.ImageFile.CopyToAsync(fileStream, cancellationToken);
                    }
                    imagePath = $"/uploads/locations/{uniqueFileName}";
                    imageExtension = extension;
                    
                }

               
                // Create the location with Guid
                var location = new ParkingLocation
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    City = dto.City,
                    TotalSlots = dto.TotalSlots,
                    ImagePath = imagePath,
                    ImageExtension = imageExtension,
                    UserId = userId ?? Guid.Empty,
                    CreatedAt = serverTime,
                    CreatedBy = userId,
                };

                _dbContext.ParkingLocations.Add(location);
                await _dbContext.SaveChangesAsync(cancellationToken);
                // handl slots insertion based on total slots in location

                for (int i = 1; i <= location.TotalSlots; i++)
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
            if (location == null)
                throw new NotFoundException("Locaiont not found");

            _dbContext.ParkingLocations.Remove(location);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var baseUrl = await _helper.GetBaseUrl();

            return await _dbContext.ParkingLocations
                //.Where(n => )
                .Include(l => l.Slots)
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    City = l.City,
                    ImageUrl = string.IsNullOrEmpty(l.ImagePath)
                            ? null
                            : $"{baseUrl.TrimEnd('/')}/{l.ImagePath.TrimStart('/')}", 
                    ImageExtension = l.ImageExtension,
                    TotalSlots = l.TotalSlots,

                    Slots = l.Slots.Select(s => new SlotResponseDto
                    {
                        Id = s.Id,
                        LocationId = s.LocationId,
                        SlotNumber = s.SlotNumber,
                        IsAvailable = s.IsAvailable
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<LocationDto?> GetLocationByIdAsync(Guid id)
        {
            var baseUrl = await _helper.GetBaseUrl();

            return await _dbContext.ParkingLocations
                .Include(l => l.Slots)
                .Where(l => l.Id == id)
                .Select(l => new LocationDto
                {
                    Id = l.Id,
                    Name = l.Name,
                    Address = l.Address,
                    City = l.City,
                    ImageUrl = string.IsNullOrEmpty(l.ImagePath)
                        ? null
                            : $"{baseUrl.TrimEnd('/')}/{l.ImagePath.TrimStart('/')}",
                    ImageExtension = l.ImageExtension,
                    TotalSlots = l.TotalSlots,

                    Slots = l.Slots.Select(s => new SlotResponseDto
                    {
                        Id = s.Id,
                        LocationId = s.LocationId,
                        SlotNumber = s.SlotNumber,
                        IsAvailable = s.IsAvailable
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public Task<List<SlotResponseDto?>> GetSlotsByLocationIdAsync(Guid id)
        {
            var slots = _dbContext.Slots
                .Where(s => s.LocationId == id)
                .Select(s => new SlotResponseDto
                {
                    Id = s.Id,
                    LocationId = s.LocationId,
                    SlotNumber = s.SlotNumber,
                    IsAvailable = s.IsAvailable
                }).ToListAsync();
            return slots;
        }

        public async Task<LocationReponse> UpdateLocationAsync(Guid id, LocationRequest dto)
        {
            var location = await _dbContext.ParkingLocations
                .Include(l => l.Slots)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (location == null)
                throw new NotFoundException("Location not found");

            // Get user ID and server time
            var userId = await _helper.GetUserIdFromToken();
            var serverTime = await _helper.GetDatabaseTime();
            string? imagePath = null;
            string? imageExtension = null;
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "locations");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var extension = Path.GetExtension(dto.ImageFile.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(fileStream);
                }
                imagePath = $"/uploads/locations/{uniqueFileName}";
                imageExtension = extension;
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(imageExtension?.ToLower()))
            {
                throw new InvalidOperationException("Only .jpg, .jpeg, and .png files are allowed.");
            }

            location.Name = dto.Name;
            location.Address = dto.Address;
            location.TotalSlots = dto.TotalSlots;
            location.City = dto.City ?? location.City;
            location.ImagePath = imagePath;
            location.ImageExtension = imageExtension;
            location.UpdatedAt = serverTime;
            location.UpdatedBy = userId;

            // Handle slot adjustments
            var currentCount = location.Slots.Count;

            if (currentCount != dto.TotalSlots)
            {
                if (dto.TotalSlots > currentCount)
                {
                    // Find the last slot number in use
                    int maxSlotNumber = location.Slots
                        .Select(s => int.TryParse(s.SlotNumber.Replace("S", ""), out var n) ? n : 0)
                        .DefaultIfEmpty(0)
                        .Max();

                    // Add new slots continuing from maxSlotNumber
                    for (int i = maxSlotNumber + 1; i <= dto.TotalSlots; i++)
                    {
                        location.Slots.Add(new Slot
                        {
                            LocationId = location.Id,
                            SlotNumber = $"S{i}",
                            IsAvailable = true
                        });
                    }
                }
                else
                {
                    // Remove slots starting from the highest SlotNumber
                    var removable = location.Slots
                        .OrderByDescending(s => int.Parse(s.SlotNumber.Replace("S", "")))
                        .Take(currentCount - dto.TotalSlots)
                        .ToList();

                    if (removable.Any(s => s.IsAvailable == false))
                        throw new ConflictException("Cannot remove occupied slots.");

                    _dbContext.Slots.RemoveRange(removable);
                }
            }

            await _dbContext.SaveChangesAsync();

            return MapToResponse(location);
        }

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