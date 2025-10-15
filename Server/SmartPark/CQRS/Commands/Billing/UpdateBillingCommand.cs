using MediatR;
using SmartPark.Dtos.Billing;

namespace SmartPark.CQRS.Commands.Billing
{
    public record UpdateBillingCommand(Guid Id, BillingRequest Request) : IRequest<BillingResponse>;

}
