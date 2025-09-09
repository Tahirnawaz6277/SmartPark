using MediatR;
using SmartPark.Dtos.Slot;

namespace SmartPark.CQRS.Queries.Slot
{
    public record GetAllSlotsQuery() : IRequest<IEnumerable<SlotResponseDto>>;

}
