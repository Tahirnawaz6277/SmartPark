using MediatR;
using SmartPark.CQRS.Commands.Location;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Location
{
    public class DeleteLocationHandler : IRequestHandler<DeleteLocationCommand, bool>
    {
        private readonly ILocationService _service;
        public DeleteLocationHandler(ILocationService service) => _service = service;

        public async Task<bool> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
            => await _service.DeleteLocationAsync(request.Id);
    }
}
