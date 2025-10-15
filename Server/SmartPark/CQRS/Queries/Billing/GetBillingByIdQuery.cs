using MediatR;
using SmartPark.Dtos.Billing;

namespace SmartPark.CQRS.Queries.Billing
{
    public record GetBillingByIdQuery(Guid Id) : IRequest<BillingDto?>;

}
