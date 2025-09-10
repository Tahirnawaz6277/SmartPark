using MediatR;
using SmartPark.CQRS.Commands.Location;
using SmartPark.Dtos.Location;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Location
{
    public class UpdateLocationHandler : IRequestHandler<UpdateLocationCommand, LocationReponse>
    {
        private readonly ILocationService _service;
        public UpdateLocationHandler(ILocationService service) => _service = service;

        public async Task<LocationReponse> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
            => await _service.UpdateLocationAsync(request.Id, request.Request);
    }
}
