using MediatR;
using SmartPark.Dtos.UserDtos;

namespace SmartPark.CQRS.Commands.User
{
    public record UpdateUserCommand(Guid Id, UpdateUserRequest RequestDto) : IRequest<UserResponseDto>;

}
