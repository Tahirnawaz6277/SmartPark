using MediatR;
using SmartPark.CQRS.Commands.Billing;
using SmartPark.Dtos.Billing;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Billing
{
    public class UpdateBillingHandler : IRequestHandler<UpdateBillingCommand, BillingResponse>
    {
        private readonly IBillingService _service;

        public UpdateBillingHandler(IBillingService service)
        {
            _service = service;
        }

        public async Task<BillingResponse> Handle(UpdateBillingCommand request, CancellationToken cancellationToken)
        {
            return await _service.UpdateBillingAsync(request.Id, request.Request, cancellationToken);
        }
    }
}
