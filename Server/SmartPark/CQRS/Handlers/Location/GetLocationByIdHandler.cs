using MediatR;
using SmartPark.CQRS.Queries.Location;
using SmartPark.Dtos.Location;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Location
{
    public class GetLocationByIdHandler : IRequestHandler<GetLocationByIdQuery, LocationResponseDto?>
    {
        private readonly ILocationService _service;
        public GetLocationByIdHandler(ILocationService service) => _service = service;

        public async Task<LocationResponseDto?> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
            => await _service.GetLocationByIdAsync(request.Id);
    }
}
