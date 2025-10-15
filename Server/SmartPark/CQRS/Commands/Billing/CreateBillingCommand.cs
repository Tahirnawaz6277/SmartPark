using MediatR;
using SmartPark.Dtos.Billing;

namespace SmartPark.CQRS.Commands.Billing
{
    public record CreateBillingCommand(BillingRequest Request) : IRequest<BillingResponse>;

}
