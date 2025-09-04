using MediatR;
using SmartPark.Dtos;

namespace SmartPark.CQRS.Queries
{
    public record LoginQuery(string Email, string Password) : IRequest<UserLoginResponse>;

}
