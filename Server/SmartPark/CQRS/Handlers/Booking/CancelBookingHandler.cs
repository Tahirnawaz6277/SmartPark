using MediatR;
using SmartPark.CQRS.Commands.Booking;
using SmartPark.Dtos.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{
    public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, BookingResponse>
    {
        private readonly IBookingService _service;
        public CancelBookingHandler(IBookingService service) => _service = service;

        public async Task<BookingResponse> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
            => await _service.CancelBookingAsync(request.Id);
    }
}
