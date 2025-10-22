using MediatR;
using SmartPark.CQRS.Queries.Location;
using SmartPark.Dtos.Location;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Location
{
    public class GetSlotsByLocationIdHandler : IRequestHandler<GetSlotsByLocationIdQuery, List<SlotResponseDto?>>
    {
        private readonly ILocationService _service;
        public GetSlotsByLocationIdHandler(ILocationService service) => _service = service;

        public async Task<List<SlotResponseDto?>> Handle(GetSlotsByLocationIdQuery request, CancellationToken cancellationToken)
            => await _service.GetSlotsByLocationIdAsync(request.Id);
    }
}
