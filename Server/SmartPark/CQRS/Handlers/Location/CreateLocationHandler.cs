using MediatR;
using SmartPark.CQRS.Commands.Location;
using SmartPark.Dtos.Location;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Location
{
    public class CreateLocationHandler : IRequestHandler<CreateLocationCommand, CreateLocationReponse>
    {
        private readonly ILocationService _service;
        public CreateLocationHandler(ILocationService service) => _service = service;

        public async Task<CreateLocationReponse> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
            => await _service.CreateLocationAsync(request.Request, cancellationToken);
    }
}
