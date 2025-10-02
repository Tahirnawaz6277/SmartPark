using MediatR;
using SmartPark.CQRS.Commands.Booking;
using SmartPark.Dtos.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{
    public class CreateBookingHandler : IRequestHandler<CreateBookingCommand, BookingResponse>
    {
        private readonly IBookingService _service;
        public CreateBookingHandler(IBookingService service) => _service = service;

        public async Task<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
            => await _service.CreateBookingAsync(request.Request, cancellationToken);
    }
}
