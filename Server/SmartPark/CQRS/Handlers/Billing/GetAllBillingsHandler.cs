using MediatR;
using SmartPark.CQRS.Queries.Billing;
using SmartPark.Dtos.Billing;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Billing
{
    public class GetAllBillingsHandler : IRequestHandler<GetAllBillingsQuery, IEnumerable<BillingDto>>
    {
        private readonly IBillingService _service;

        public GetAllBillingsHandler(IBillingService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<BillingDto>> Handle(GetAllBillingsQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAllBillingsAsync(cancellationToken);
        }
    }
}
