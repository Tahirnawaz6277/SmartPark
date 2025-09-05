using MediatR;
using SmartPark.CQRS.Queries.Location;
using SmartPark.Dtos.Location;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Location
{
    public class GetAllLocationsHandler : IRequestHandler<GetAllLocationsQuery, IEnumerable<LocationResponseDto>>
    {
        private readonly ILocationService _service;
        public GetAllLocationsHandler(ILocationService service) => _service = service;

        public async Task<IEnumerable<LocationResponseDto>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
            => await _service.GetAllLocationsAsync();
    }
}
