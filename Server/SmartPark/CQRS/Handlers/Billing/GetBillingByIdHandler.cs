using MediatR;
using SmartPark.CQRS.Queries.Billing;
using SmartPark.Dtos.Billing;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Billing
{
    public class GetBillingByIdHandler : IRequestHandler<GetBillingByIdQuery, BillingDto?>
    {
        private readonly IBillingService _service;

        public GetBillingByIdHandler(IBillingService service)
        {
            _service = service;
        }

        public async Task<BillingDto?> Handle(GetBillingByIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetBillingByIdAsync(request.Id, cancellationToken);
        }
    }
}
