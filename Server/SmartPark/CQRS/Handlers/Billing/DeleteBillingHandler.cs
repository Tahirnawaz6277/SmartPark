using MediatR;
using SmartPark.CQRS.Commands.Billing;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Billing
{
    public class DeleteBillingHandler : IRequestHandler<DeleteBillingCommand, bool>
    {
        private readonly IBillingService _service;

        public DeleteBillingHandler(IBillingService service)
        {
            _service = service;
        }

        public async Task<bool> Handle(DeleteBillingCommand request, CancellationToken cancellationToken)
        {
            return await _service.DeleteBillingAsync(request.Id, cancellationToken);
        }
    }
}
