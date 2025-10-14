using MediatR;
using SmartPark.Dtos.Booking;

namespace SmartPark.CQRS.Commands.Booking
{
    public record CancelBookingCommand(Guid Id):IRequest<BookingResponse>;
    
}
