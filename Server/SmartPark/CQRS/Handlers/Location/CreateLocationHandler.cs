using MediatR;
using SmartPark.CQRS.Commands.Location;
using SmartPark.Dtos.Location;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Location
{
    public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, LocationResponseDto>
    {
        private readonly ILocationService _service;
        public CreateLocationHandler(ILocationService service) => _service = service;

        public async Task<LocationResponseDto> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
            => await _service.CreateLocationAsync(request.Request);
    }
}
