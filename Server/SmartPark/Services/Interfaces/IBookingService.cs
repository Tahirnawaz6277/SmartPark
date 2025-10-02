using SmartPark.Dtos.Booking;

namespace SmartPark.Services.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(BookingRequest dto, CancellationToken cancellationToken);
        Task<BookingDto?> GetBookingByIdAsync(Guid id);
        Task<BookingHistoryDto?> GetBookingHistoryByIdAsync(Guid historyId);
        Task<IEnumerable<BookingHistoryDto?>> GetBookingHistoriesAsync(Guid? bookingId);
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingResponse> UpdateBookingAsync(Guid id, BookingRequest dto);
        Task<bool> DeleteBookingAsync(Guid id);
    }
}
