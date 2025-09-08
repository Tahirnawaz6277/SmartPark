using MediatR;

namespace SmartPark.CQRS.Commands.Location
{
    public record DeleteLocationCommand(Guid Id) : IRequest<bool>;

}
