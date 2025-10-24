using MediatR;
using SmartPark.Dtos.Booking;

namespace SmartPark.CQRS.Queries.Booking
{
    public record GetMyBookingsQuery() : IRequest<IEnumerable<BookingDto>>;

}
