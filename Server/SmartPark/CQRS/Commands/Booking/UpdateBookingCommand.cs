using MediatR;
using SmartPark.Dtos.Booking;

namespace SmartPark.CQRS.Commands.Booking
{
    public record UpdateBookingCommand(Guid Id, BookingRequest Request) : IRequest<BookingResponse>;

}
