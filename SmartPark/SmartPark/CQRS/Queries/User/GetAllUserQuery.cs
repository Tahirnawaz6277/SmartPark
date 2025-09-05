using MediatR;
using SmartPark.Dtos.UserDtos;

namespace SmartPark.CQRS.Queries.User
{
    public record GetAllUserQuery() : IRequest<List<UserDto>>;
}
