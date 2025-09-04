using MediatR;
using SmartPark.Dtos;

namespace SmartPark.CQRS.Commands
{
    public record UpdateUserCommand(Guid Id, UpdateUserRequest RequestDto) : IRequest<UserResponseDto>;

}
