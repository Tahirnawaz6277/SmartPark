using MediatR;
using SmartPark.CQRS.Queries.Booking;
using SmartPark.Dtos.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{
    public class GetBookingHistoriesHandler : IRequestHandler<GetBookingHistoriesQuery, IEnumerable<BookingHistoryDto>>
    {
        private readonly IBookingService _service;
        public GetBookingHistoriesHandler(IBookingService service) => _service = service;

        public async Task<IEnumerable<BookingHistoryDto?>> Handle(GetBookingHistoriesQuery request, CancellationToken cancellationToken)
            => await _service.GetBookingHistoriesAsync(request.BookingId);
    }
}
