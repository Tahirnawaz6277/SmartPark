using MediatR;
using SmartPark.CQRS.Queries.Booking;
using SmartPark.Dtos.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{
    public class GetUnpaidBookingsHandler : IRequestHandler<GetUnpaidBookingsQuery, IEnumerable<BookingDto>>
    {
        private readonly IBookingService _service;
        public GetUnpaidBookingsHandler(IBookingService service) => _service = service;

        public async Task<IEnumerable<BookingDto>> Handle(GetUnpaidBookingsQuery request, CancellationToken cancellationToken)
            => await _service.GetUnpaidBookingsAsync();
    }
}
