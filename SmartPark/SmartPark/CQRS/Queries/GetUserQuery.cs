using MediatR;
using SmartPark.Dtos;

namespace SmartPark.CQRS.Queries
{
    public record GetUserQuery(Guid Id) : IRequest<UserDto?>;

}
