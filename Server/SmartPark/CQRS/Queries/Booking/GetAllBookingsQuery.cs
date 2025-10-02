using MediatR;
using SmartPark.Dtos.Booking;

namespace SmartPark.CQRS.Queries.Booking
{
    public record GetAllBookingsQuery() : IRequest<IEnumerable<BookingDto>>;

}
