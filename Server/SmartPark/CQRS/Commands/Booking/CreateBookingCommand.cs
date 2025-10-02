using MediatR;
using SmartPark.Dtos.Booking;

namespace SmartPark.CQRS.Commands.Booking
{
    public record CreateBookingCommand(BookingRequest Request) : IRequest<BookingResponse>;

}
