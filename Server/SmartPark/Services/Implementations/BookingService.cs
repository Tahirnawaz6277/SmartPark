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
                    SlotId = dto.SlotId
                };

                _dbContext.Bookings.Add(booking);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var bookingHistory = new BookingHistory
                {
                    StatusSnapshot = booking.Status,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    TimeStamp = serverTime,
                    SlotId = dto.SlotId,
                    BookingId = booking.Id,
                    UserId = userId ?? Guid.Empty
                };

                _dbContext.BookingHistories.Add(bookingHistory);

                var slot = await _dbContext.Slots.FindAsync(dto.SlotId);
                if (slot == null)
                    throw new NotFoundException("Slot not found");
                 if (slot.IsAvailable ==false)
                    throw new ConflictException("This Slot is already Booked");
                
                slot.IsAvailable = false;
                _dbContext.Slots.Update(slot);

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
            var booking = await _dbContext.Bookings.FindAsync(id);
            if (booking == null)
                throw new NotFoundException("Booking not found");

            //_dbContext.Bookings.Remove(booking);
            booking.IsDeleted = true;

            //  free the slot again
            var slot = await _dbContext.Slots.FindAsync(booking.SlotId);
            if (slot != null)
            {
                slot.IsAvailable = true;
                _dbContext.Slots.Update(slot);
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
                              SlotId = b.SlotId,
                              SlotNumber = b.Slot != null ? b.Slot.SlotNumber : null
                          }).ToListAsync();
        }

        //  Get By Id
        public async Task<BookingDto?> GetBookingByIdAsync(Guid id)
        {
            var booking = await _dbContext.Bookings
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
                              SlotId = b.SlotId,
                              SlotNumber = b.Slot != null ? b.Slot.SlotNumber : null
                          }).FirstOrDefaultAsync();


            if (booking == null)
                throw new NotFoundException("booking not found");

            return booking;
        }

        //cancell booking
        public async Task<BookingResponse> CancelBookingAsync(Guid id)
        {
            var booking = await _dbContext.Bookings.FindAsync(id);
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

            //  insert new history snapshot
            var history = new BookingHistory
            {
                StatusSnapshot = booking.Status,
                TimeStamp = serverTime,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                SlotId = booking.SlotId,
                BookingId = booking.Id,
                UserId = booking.UserId
            };
            _dbContext.BookingHistories.Add(history);

            //  update slot availability (if slot changed)
            var slot = await _dbContext.Slots.FindAsync(booking.SlotId);
            if (slot != null)
            {
                //if (slot.IsAvailable == false)
                //    throw new ConflictException("This booking is already Cancelled");
                slot.IsAvailable = false;
                _dbContext.Slots.Update(slot);
            }

            await _dbContext.SaveChangesAsync();
            return MapToResponse(booking);
        }
        //  Update Booking
        public async Task<BookingResponse> UpdateBookingAsync(Guid id, BookingRequest dto)
        {
            var booking = await _dbContext.Bookings.FindAsync(id);
            if (booking == null)
                throw new NotFoundException("Booking not found");

            var serverTime = await _helper.GetDatabaseTime();

            // update booking
            booking.UpdatedAt = serverTime;
            booking.StartTime = dto.StartTime;
            booking.EndTime = dto.EndTime;
            booking.SlotId = dto.SlotId;

            _dbContext.Bookings.Update(booking);

            //  insert new history snapshot
            var history = new BookingHistory
            {
                StatusSnapshot = booking.Status,
                TimeStamp = serverTime,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                SlotId = dto.SlotId,
                BookingId = booking.Id,
                UserId = booking.UserId
            };
            _dbContext.BookingHistories.Add(history);

            //  update slot availability (if slot changed)
            var slot = await _dbContext.Slots.FindAsync(dto.SlotId);
            if (slot != null)
            {
                if (slot.IsAvailable == false)
                    throw new ConflictException("This Slot is already Booked");
                slot.IsAvailable = false;
                _dbContext.Slots.Update(slot);
            }

            await _dbContext.SaveChangesAsync();
            return MapToResponse(booking);
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
                SlotId = booking.SlotId
            };
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
                    SlotId = b.SlotId,
                    SlotNumber = b.Slot != null ? b.Slot.SlotNumber : null
                })
                .ToListAsync();
        }



    }
}
