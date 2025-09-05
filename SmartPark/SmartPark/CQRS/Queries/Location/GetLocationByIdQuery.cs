using MediatR;
using SmartPark.Dtos.Location;

namespace SmartPark.CQRS.Queries.Location
{
    public record GetLocationByIdQuery(Guid Id) : IRequest<LocationResponseDto?>;

}
