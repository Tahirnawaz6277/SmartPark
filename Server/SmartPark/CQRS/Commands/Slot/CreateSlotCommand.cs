using MediatR;
using SmartPark.Dtos.Slot;

namespace SmartPark.CQRS.Commands.Slot
{
    public record CreateSlotCommand(SlotRequestDto Dto) : IRequest<SlotResponseDto>;

}
