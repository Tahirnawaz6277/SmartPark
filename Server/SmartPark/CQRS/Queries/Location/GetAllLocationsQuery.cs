using MediatR;
using SmartPark.Dtos.Location;

namespace SmartPark.CQRS.Queries.Location
{
    public record GetAllLocationsQuery() : IRequest<IEnumerable<LocationResponseDto>>;

}
