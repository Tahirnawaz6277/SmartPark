using MediatR;

namespace SmartPark.CQRS.Commands.Booking
{
    public record DeleteBookingCommand(Guid Id) : IRequest<bool>;

}
