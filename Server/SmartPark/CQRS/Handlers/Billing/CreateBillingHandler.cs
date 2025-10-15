using MediatR;
using SmartPark.CQRS.Commands.Billing;
using SmartPark.Dtos.Billing;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Billing
{
    public class CreateBillingHandler : IRequestHandler<CreateBillingCommand, BillingResponse>
    {
        private readonly IBillingService _service;

        public CreateBillingHandler(IBillingService service)
        {
            _service = service;
        }

        public async Task<BillingResponse> Handle(CreateBillingCommand request, CancellationToken cancellationToken)
        {
            return await _service.CreateBillingAsync(request.Request, cancellationToken);
        }
    }
}
