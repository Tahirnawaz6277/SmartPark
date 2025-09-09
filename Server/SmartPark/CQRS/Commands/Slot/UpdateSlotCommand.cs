using MediatR;
using SmartPark.Dtos.Slot;

namespace SmartPark.CQRS.Commands.Slot
{
    public record UpdateSlotCommand(Guid Id, SlotRequestDto Dto) : IRequest<SlotResponseDto>;

}
