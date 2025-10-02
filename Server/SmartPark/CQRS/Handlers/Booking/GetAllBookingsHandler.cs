using MediatR;
using SmartPark.CQRS.Queries.Booking;
using SmartPark.Dtos.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{
    public class GetAllBookingsHandler : IRequestHandler<GetAllBookingsQuery, IEnumerable<BookingDto>>
    {
        private readonly IBookingService _service;
        public GetAllBookingsHandler(IBookingService service) => _service = service;

        public async Task<IEnumerable<BookingDto>> Handle(GetAllBookingsQuery request, CancellationToken cancellationToken)
            => await _service.GetAllBookingsAsync();
    }
}
