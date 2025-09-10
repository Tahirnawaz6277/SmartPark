using MediatR;
using SmartPark.Dtos.Location;

namespace SmartPark.CQRS.Commands.Location
{
    public record CreateLocationCommand(CreateLocationRequest Request) : IRequest<CreateLocationReponse>;

}
