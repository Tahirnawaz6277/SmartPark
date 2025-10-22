using MediatR;
using SmartPark.Dtos.Location;

namespace SmartPark.CQRS.Queries.Location
{ 
    public record GetSlotsByLocationIdQuery(Guid Id) : IRequest<List<SlotResponseDto?>>;

}
