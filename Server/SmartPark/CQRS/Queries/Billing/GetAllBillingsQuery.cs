using MediatR;
using SmartPark.Dtos.Billing;

namespace SmartPark.CQRS.Queries.Billing
{
    public record GetAllBillingsQuery() : IRequest<IEnumerable<BillingDto>>;

}
