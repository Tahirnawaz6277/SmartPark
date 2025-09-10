using MediatR;
using SmartPark.Dtos.Location;

namespace SmartPark.CQRS.Commands.Location
{
    public record UpdateLocationCommand(Guid Id, CreateLocationRequest Request) : IRequest<CreateLocationReponse>;

}
