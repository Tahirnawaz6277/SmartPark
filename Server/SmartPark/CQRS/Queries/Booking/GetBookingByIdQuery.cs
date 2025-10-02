using MediatR;
using SmartPark.Dtos.Booking;

namespace SmartPark.CQRS.Queries.Booking
{
    public record GetBookingByIdQuery(Guid Id) : IRequest<BookingDto?>;

}
