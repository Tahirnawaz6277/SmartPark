using MediatR;
using SmartPark.Dtos.UserDtos;

namespace SmartPark.CQRS.Commands.User
{
    public record CreateUserCommand(RegistrationRequest RequestDto) : IRequest<RegistrationResponse>;
}
