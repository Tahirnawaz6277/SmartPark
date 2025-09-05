using MediatR;
using SmartPark.Dtos.Location;

namespace SmartPark.CQRS.Commands.Location
{
    public record CreateLocationCommand(LocationRequestDto Request) : IRequest<LocationResponseDto>;

}
