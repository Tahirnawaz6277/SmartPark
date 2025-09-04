using MediatR;
using SmartPark.Dtos;

namespace SmartPark.CQRS.Queries
{
    public record GetAllUserQuery() : IRequest<List<UserDto>>;
}
