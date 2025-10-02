using MediatR;
using SmartPark.CQRS.Queries.Booking;
using SmartPark.Dtos.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{
    public class GetBookingHistoryByIdHandler : IRequestHandler<GetBookingHistoryByIdQuery, BookingHistoryDto?>
    {
        private readonly IBookingService _service;
        public GetBookingHistoryByIdHandler(IBookingService service) => _service = service;

        public async Task<BookingHistoryDto?> Handle(GetBookingHistoryByIdQuery request, CancellationToken cancellationToken)
            => await _service.GetBookingHistoryByIdAsync(request.Id);
    }
}
