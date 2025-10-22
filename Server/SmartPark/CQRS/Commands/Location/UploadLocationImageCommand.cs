using MediatR;

namespace SmartPark.CQRS.Commands.Location
{
    public class UploadLocationImageCommand : IRequest<string>
    {
        public Guid LocationId { get; set; }
        public IFormFile File { get; set; }
    }

}
