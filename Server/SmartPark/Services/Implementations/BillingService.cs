using Microsoft.EntityFrameworkCore;
using SmartPark.Data.Contexts;
using SmartPark.Dtos.Billing;
using SmartPark.Exceptions;
using SmartPark.Models;
using SmartPark.Services.Interfaces;

namespace SmartPark.Services.Implementations
{
    public class BillingService : IBillingService
    {
        private readonly ParkingDbContext _dbContext;
        private readonly IHelper _helper;

        public BillingService(ParkingDbContext dbContext, IHelper helper)
        {
            _dbContext = dbContext;
            _helper = helper;
        }

        public async Task<BillingResponse> CreateBillingAsync(BillingRequest request, CancellationToken cancellationToken)
        {
            var existingBilling = await _dbContext.Billings.AnyAsync(b => b.BookingId == request.BookingId);
            if (existingBilling)
            {
                throw new ConflictException("Specified booking is already paid");
            }

            var billing = new Billing
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                PaymentStatus = "Paid",
                PaymentMethod = "Cash",
                TimeStamp = DateTime.UtcNow,
                BookingId = request.BookingId
            };

            _dbContext.Billings.Add(billing);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BillingResponse
            {
                Id = billing.Id,
                Amount = billing.Amount,
                PaymentStatus = billing.PaymentStatus!,
                PaymentMethod = billing.PaymentMethod!,
                TimeStamp = billing.TimeStamp ?? DateTime.UtcNow,
                BookingId = billing.BookingId
            };
        }

        public async Task<BillingResponse> UpdateBillingAsync(Guid id, BillingRequest request, CancellationToken cancellationToken)
        {
            var billing = await _dbContext.Billings.FindAsync(new object[] { id }, cancellationToken);
            if (billing == null) throw new NotFoundException($"Billing with Id {id} not found.");

            billing.Amount = request.Amount;
            //billing.PaymentStatus = request.PaymentStatus;
            //billing.PaymentMethod = "Cash";
            billing.BookingId = request.BookingId;
            billing.TimeStamp = DateTime.UtcNow;

            _dbContext.Billings.Update(billing);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new BillingResponse
            {
                Id = billing.Id,
                Amount = billing.Amount,
                PaymentStatus = billing.PaymentStatus!,
                PaymentMethod = billing.PaymentMethod!,
                TimeStamp = billing.TimeStamp ?? DateTime.UtcNow,
                BookingId = billing.BookingId
            };
        }

        public async Task<bool> DeleteBillingAsync(Guid id, CancellationToken cancellationToken)
        {
            var billing = await _dbContext.Billings.FindAsync(new object[] { id }, cancellationToken);
            if (billing == null) return false;

            //_dbContext.Billings.Remove(billing);
            billing.IsDeleted = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IEnumerable<BillingDto>> GetAllBillingsAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Billings
                .AsNoTracking()
                .Select(b => new BillingDto
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    PaymentStatus = b.PaymentStatus,
                    PaymentMethod = b.PaymentMethod,
                    TimeStamp = b.TimeStamp,
                    BookingId = b.BookingId,
                    SlotNumbers = b.Booking.Slots.Select(s => s.SlotNumber).ToList(),
                    UserName = b.Booking.User.Name,
                    LocationName = b.Booking.Slots
                        .OrderBy(s => s.SlotNumber)
                        .Select(s => s.Location.Name)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<BillingDto?> GetBillingByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Billings
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BillingDto
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    PaymentStatus = b.PaymentStatus,
                    PaymentMethod = b.PaymentMethod,
                    TimeStamp = b.TimeStamp,
                    BookingId = b.BookingId,
                    SlotNumbers = b.Booking.Slots.Select(s => s.SlotNumber).ToList(),
                    UserName = b.Booking.User.Name,
                    LocationName = b.Booking.Slots
                        .OrderBy(s => s.SlotNumber)
                        .Select(s => s.Location.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<BillingDto>> GetMyBillingsAsync(CancellationToken cancellationToken)
        {
            var userId = await _helper.GetUserIdFromToken();

            return await _dbContext.Billings
                .AsNoTracking()
                .Where(b => b.Booking.UserId == userId)
                .Select(b => new BillingDto
                {
                    Id = b.Id,
                    Amount = b.Amount,
                    PaymentStatus = b.PaymentStatus,
                    PaymentMethod = b.PaymentMethod,
                    TimeStamp = b.TimeStamp,
                    BookingId = b.BookingId,
                    UserName = b.Booking.User.Name,
                    SlotNumbers = b.Booking.Slots.Select(s => s.SlotNumber).ToList(),
                    LocationName = b.Booking.Slots
                        .OrderBy(s => s.SlotNumber)
                        .Select(s => s.Location.Name)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
