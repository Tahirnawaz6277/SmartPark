using MediatR;
using SmartPark.CQRS.Commands.User;
using SmartPark.Dtos.UserDtos;
using SmartPark.Services.Interfaces;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, RegistrationResponse>
{
    private readonly IUserService _userService;

    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<RegistrationResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _userService.UpdateUserAsync(request.Id, request.RequestDto);
    }
}
