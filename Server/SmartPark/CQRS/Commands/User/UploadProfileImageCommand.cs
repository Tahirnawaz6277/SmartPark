using MediatR;

namespace SmartPark.CQRS.Commands.User
{
    public class UploadProfileImageCommand : IRequest<string>
    {
        public Guid UserId { get; set; }
        public IFormFile File { get; set; }
    }
}
