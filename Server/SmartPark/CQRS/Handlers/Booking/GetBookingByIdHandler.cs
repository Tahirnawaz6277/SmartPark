using MediatR;
using SmartPark.CQRS.Queries.Booking;
using SmartPark.Dtos.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{
    public class GetBookingByIdHandler : IRequestHandler<GetBookingByIdQuery, BookingDto?>
    {
        private readonly IBookingService _service;
        public GetBookingByIdHandler(IBookingService service) => _service = service;

        public async Task<BookingDto?> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
            => await _service.GetBookingByIdAsync(request.Id);
    }
}
