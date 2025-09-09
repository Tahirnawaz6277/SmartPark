using MediatR;

namespace SmartPark.CQRS.Commands.Slot
{
    public record DeleteSlotCommand(Guid Id) : IRequest<bool>;

}
