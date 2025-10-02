using MediatR;
using SmartPark.Dtos.Booking;

namespace SmartPark.CQRS.Queries.Booking
{
    public record GetBookingHistoryByIdQuery(Guid Id) : IRequest<BookingHistoryDto?>;

}
