using MediatR;
using SmartPark.Dtos.UserDtos;

namespace SmartPark.CQRS.Queries.User
{
    public record GetUserQuery(Guid Id) : IRequest<UserDto?>;

}
