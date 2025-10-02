using MediatR;
using SmartPark.CQRS.Commands.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{
    public class DeleteBookingHandler : IRequestHandler<DeleteBookingCommand, bool>
    {
        private readonly IBookingService _service;
        public DeleteBookingHandler(IBookingService service) => _service = service;

        public async Task<bool> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
            => await _service.DeleteBookingAsync(request.Id);
    }
}
