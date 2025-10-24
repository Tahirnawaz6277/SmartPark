using MediatR;
using SmartPark.CQRS.Queries.Booking;
using SmartPark.Dtos.Booking;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Booking
{

    public class GetMyBookingsHandler : IRequestHandler<GetMyBookingsQuery, IEnumerable<BookingDto>>
    {
        private readonly IBookingService _service;
        public GetMyBookingsHandler(IBookingService service) => _service = service;

        public async Task<IEnumerable<BookingDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
            => await _service.GetMyBookingsAsync();
    }

}
