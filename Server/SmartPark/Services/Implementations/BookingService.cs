using Hangfire;
using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Dtos.Booking;
using SmartPark.Exceptions;
using SmartPark.Models;
using SmartPark.Services.Interfaces;

namespace SmartPark.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly ParkingDbContext _dbContext;
        private readonly IHelper _helper;

        public BookingService(ParkingDbContext dbContext, IHelper helper)
        {
            _dbContext = dbContext;
            _helper = helper;
        }

        public async Task<BookingResponse> CreateBookingAsync(BookingRequest dto, CancellationToken cancellationToken)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var userId = await _helper.GetUserIdFromToken();
                var serverTime = await _helper.GetDatabaseTime();

                var booking = new Booking
                {
                    Status = "Booked",
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    CreatedAt = serverTime,
                    UserId = userId ?? Guid.Empty,
                };

                _dbContext.Bookings.Add(booking);
                await _dbContext.SaveChangesAsync(cancellationToken);

                // Handle multiple slots
                foreach (var slotId in dto.SlotIds)
                {
                    await BookSlotAsync(slotId, booking.Id);
                    var delay = booking.EndTime - serverTime;
                    if (delay.TotalSeconds < 0) delay = TimeSpan.Zero;

                    BackgroundJob.Schedule(() => ReleaseSlot(slotId), delay);

                    // Add a booking history per slot
                    var bookingHistory = new BookingHistory
                    {
                        StatusSnapshot = booking.Status,
                        StartTime = booking.StartTime,
                        EndTime = booking.EndTime,
                        TimeStamp = serverTime,
                        SlotId = slotId,
                        BookingId = booking.Id,
                        UserId = userId ?? Guid.Empty
                    };

                    _dbContext.BookingHistories.Add(bookingHistory);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return MapToResponse(booking);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<bool> DeleteBookingAsync(Guid id)
        {
            var booking = await _dbContext.Bookings
                  .Include(b => b.Slots)
                  .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null)
                throw new NotFoundException("Booking not found");

            //_dbContext.Bookings.Remove(booking);
            booking.IsDeleted = true;

            // Free all related slots
            foreach (var slot in booking.Slots)
            {
                slot.IsAvailable = true;
                slot.BookingId = null;
                //_dbContext.Slots.Update(slot);
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        //  Get All for admin
        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            return await _dbContext.Bookings
                             .AsNoTracking()
                           .Select(b => new BookingDto
                           {
                               Id = b.Id,
                               Status = b.Status,
                               StartTime = b.StartTime,
                               EndTime = b.EndTime,
                               UserId = b.UserId,
                               UserName = b.User != null ? b.User.Name : null,
                               Slots = b.Slots.Select(s => new SlotDto
                               {
                                   SlotId = s.Id,
                                   SlotNumber = s.SlotNumber,
                                   LocationName = s.Location != null ? s.Location.Name : ""
                               }).ToList()

                           }).ToListAsync();
        }

        //  Get By Id
        public async Task<BookingDto?> GetBookingByIdAsync(Guid id)
        {
            var booking = await _dbContext.Bookings
                .Include(b => b.User)
                .Include(b => b.Slots)
                .ThenInclude(s => s.Location)
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    Status = b.Status,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    UserId = b.UserId,
                    UserName = b.User != null ? b.User.Name : null,
                    Slots = b.Slots.Select(s => new SlotDto
                    {
                        SlotId = s.Id,
                        SlotNumber = s.SlotNumber,
                        LocationName = s.Location != null ? s.Location.Name : ""
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (booking == null)
                throw new NotFoundException("Booking not found");

            return booking;
        }


        //cancell booking
        public async Task<BookingResponse> CancelBookingAsync(Guid id)
        {
            var booking = await _dbContext.Bookings
                     .Include(b => b.Slots)
                     .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                throw new NotFoundException("Booking not found");

            var serverTime = await _helper.GetDatabaseTime();
            var userId = await _helper.GetUserIdFromToken();
            if (booking.Status == "Cancelled")
            {
                throw new ConflictException("This booking is already Cancelled");
            }

            // update booking
            booking.Status = "Cancelled";
            booking.CancelledAt = serverTime;
            booking.CancelledBy = userId;

            _dbContext.Bookings.Update(booking);
            // free all slots associated with this booking
            foreach (var slot in booking.Slots)
            {
                slot.IsAvailable = true;
                slot.BookingId = null;
                _dbContext.Slots.Update(slot);

                // add cancellation history
                _dbContext.BookingHistories.Add(new BookingHistory
                {
                    StatusSnapshot = "Cancelled",
                    TimeStamp = serverTime,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    SlotId = slot.Id,
                    BookingId = booking.Id,
                    UserId = booking.UserId
                });
            }
            await _dbContext.SaveChangesAsync();
            return MapToResponse(booking);
        }

        //  Update Booking
        public async Task<BookingResponse> UpdateBookingAsync(Guid id, BookingRequest dto)
        {
            var booking = await _dbContext.Bookings
                .Include(b => b.Slots)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                throw new NotFoundException("Booking not found");

            var serverTime = await _helper.GetDatabaseTime();

            // Free previous slots
            foreach (var slot in booking.Slots)
            {
                slot.IsAvailable = true;
                slot.BookingId = null;
                _dbContext.Slots.Update(slot);
            }

            // Assign new slots
            var newSlots = await _dbContext.Slots
                .Where(s => dto.SlotIds.Contains(s.Id))
                .ToListAsync();

            foreach (var slot in newSlots)
            {
                if (!slot.IsAvailable)
                    throw new ConflictException($"Slot {slot.SlotNumber} is already booked");

                slot.IsAvailable = false;
                slot.BookingId = booking.Id;
            }

            booking.StartTime = dto.StartTime;
            booking.EndTime = dto.EndTime;
            booking.UpdatedAt = serverTime;

            _dbContext.Bookings.Update(booking);
            await _dbContext.SaveChangesAsync();

            return MapToResponse(booking);
        }

        // booking history get methods
        public async Task<IEnumerable<BookingHistoryDto?>> GetBookingHistoriesAsync(Guid? bookingId)
        {
            var query = _dbContext.BookingHistories.AsNoTracking().AsQueryable();
            if (bookingId != null)
            {
                query = query.Where(h => h.BookingId != null && h.BookingId == bookingId.Value);
            }

            var histories = await query
                .Select(h => new BookingHistoryDto
                {
                    Id = h.Id,
                    StatusSnapshot = h.StatusSnapshot,
                    TimeStamp = h.TimeStamp,
                    SlotId = h.SlotId,
                    BookingId = h.BookingId,
                    UserId = h.UserId
                }).ToListAsync();

            return histories;
        }

        // get booking history by id
        public async Task<BookingHistoryDto?> GetBookingHistoryByIdAsync(Guid historyId)
        {
            var history = await _dbContext.BookingHistories
                .AsNoTracking()
                .Where(h => h.Id == historyId)
                .Select(h => new BookingHistoryDto
                {
                    Id = h.Id,
                    StatusSnapshot = h.StatusSnapshot,
                    TimeStamp = h.TimeStamp,
                    SlotId = h.SlotId,
                    BookingId = h.BookingId,
                    UserId = h.UserId
                })
                .FirstOrDefaultAsync();

            if (history == null)
                throw new NotFoundException("BookingHistory not found");

            return history;
        }

        //get booking with unpaid bills for billing dropdown

        public async Task<IEnumerable<BookingDto>> GetUnpaidBookingsAsync()
        {
            var paidBookingIds = await _dbContext.Billings
                .Where(b => b.PaymentStatus.ToLower() == "paid")
                .Select(b => b.BookingId)
                .ToListAsync();

            return await _dbContext.Bookings
                .Where(b => !paidBookingIds.Contains(b.Id)
                         && b.Status != null
                         && b.Status.ToLower() != "Cancelled")
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    Status = b.Status,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    UserId = b.UserId,
                    UserName = b.User != null ? b.User.Name : null,
                    Slots = b.Slots.Select(s => new SlotDto
                    {
                        SlotId = s.Id,
                        SlotNumber = s.SlotNumber,
                        LocationName = s.Location != null ? s.Location.Name : ""
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<IEnumerable<BookingDto>> GetMyBookingsAsync()
        {
            var userId = await _helper.GetUserIdFromToken();

            return await _dbContext.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Slots)
                .ThenInclude(s => s.Location)
                .AsNoTracking()
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    Status = b.Status,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Slots = b.Slots.Select(s => new SlotDto
                    {
                        SlotId = s.Id,
                        SlotNumber = s.SlotNumber,
                        LocationName = s.Location != null ? s.Location.Name : ""
                    }).ToList()
                })
                .ToListAsync();
        }


        // helper private methods
        public async Task ReleaseSlot(Guid slotId)
        {
            var slot = await _dbContext.Slots.FirstOrDefaultAsync(s => s.Id == slotId);
            if (slot == null)
                throw new NotFoundException("Slot not found");

            if (slot.IsAvailable)
                return;

            slot.IsAvailable = true;
            _dbContext.Slots.Update(slot);
            await _dbContext.SaveChangesAsync();
        }


        private async Task BookSlotAsync(Guid slotId, Guid bookingId)
        {
            var slot = await _dbContext.Slots.FirstOrDefaultAsync(s => s.Id == slotId);
            if (slot == null)
                throw new NotFoundException("Slot not found");

            if (!slot.IsAvailable)
                throw new ConflictException("Slot is already booked");

            slot.IsAvailable = false;
            slot.BookingId = bookingId;

            _dbContext.Slots.Update(slot);
        }


        // Mapping helpers
        private BookingResponse MapToResponse(Booking booking)
        {
            return new BookingResponse
            {
                Id = booking.Id,
                Status = booking.Status,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                UserId = booking.UserId,
                SlotIds = booking.Slots.Select(s => s.Id).ToList()
            };
        }
    }
}
