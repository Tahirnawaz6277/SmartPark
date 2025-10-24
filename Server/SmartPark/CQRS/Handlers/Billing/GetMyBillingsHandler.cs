using MediatR;
using SmartPark.CQRS.Queries.Billing;
using SmartPark.Dtos.Billing;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Billing
{
    public class GetMyBillingsHandler : IRequestHandler<GetMyBillingsQuery, IEnumerable<BillingDto>>
    {
        private readonly IBillingService _service;

        public GetMyBillingsHandler(IBillingService service)
        {
            _service = service;
        }

        public async Task<IEnumerable<BillingDto>> Handle(GetMyBillingsQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetMyBillingsAsync(cancellationToken);
        }
    }
}
