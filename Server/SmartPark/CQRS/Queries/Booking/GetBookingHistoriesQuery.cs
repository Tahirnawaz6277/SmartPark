using MediatR;
using SmartPark.Dtos.Booking;

namespace SmartPark.CQRS.Queries.Booking
{
    public record GetBookingHistoriesQuery(Guid? BookingId) : IRequest<IEnumerable<BookingHistoryDto>>;

}
