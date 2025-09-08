using MediatR;

namespace SmartPark.CQRS.Commands
{
    public record DeleteUserCommad(Guid Id) : IRequest<Guid>;
}
