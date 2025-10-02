using MediatR;
using SmartPark.CQRS.Commands.Booking;
using SmartPark.Dtos.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{
    public class UpdateBookingHandler : IRequestHandler<UpdateBookingCommand, BookingResponse>
    {
        private readonly IBookingService _service;
        public UpdateBookingHandler(IBookingService service) => _service = service;

        public async Task<BookingResponse> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
            => await _service.UpdateBookingAsync(request.Id, request.Request);
    }

}
