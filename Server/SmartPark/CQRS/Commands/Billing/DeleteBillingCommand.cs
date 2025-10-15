using MediatR;

namespace SmartPark.CQRS.Commands.Billing
{
    public record DeleteBillingCommand(Guid Id) : IRequest<bool>;

}
