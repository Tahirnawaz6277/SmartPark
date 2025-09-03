using MediatR;
using SmartPark.Dtos;

namespace SmartPark.CQRS.Commands
{
    public record CreateUserCommand(UserRequestDto RequestDto) : IRequest<UserResponseDto>;
}
