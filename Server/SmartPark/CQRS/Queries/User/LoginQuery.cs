using MediatR;
using SmartPark.Dtos.UserDtos;

namespace SmartPark.CQRS.Queries.User
{
    public record LoginQuery(string Email, string Password) : IRequest<LoginResponse>;

}
