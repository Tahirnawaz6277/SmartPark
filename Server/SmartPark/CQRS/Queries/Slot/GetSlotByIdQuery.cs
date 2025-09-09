using MediatR;
using SmartPark.Dtos.Slot;

namespace SmartPark.CQRS.Queries.Slot
{
    public record GetSlotByIdQuery(Guid Id) : IRequest<SlotResponseDto?>;

}
