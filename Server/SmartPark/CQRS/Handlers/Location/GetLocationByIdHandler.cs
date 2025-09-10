using MediatR;
using SmartPark.CQRS.Queries.Location;
using SmartPark.Dtos.Location;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Location
{
    public class GetLocationByIdHandler : IRequestHandler<GetLocationByIdQuery, LocationDto?>
    {
        private readonly ILocationService _service;
        public GetLocationByIdHandler(ILocationService service) => _service = service;

        public async Task<LocationDto?> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
            => await _service.GetLocationByIdAsync(request.Id);
    }
}
