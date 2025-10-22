using MediatR;
using SmartPark.CQRS.Commands.Location;
using SmartPark.Data.Contexts;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.Location
{
    public class UploadLocationImageHandler : IRequestHandler<UploadLocationImageCommand, string>
    {
        private readonly ParkingDbContext _context;
        private readonly IFileService _fileService;

        public UploadLocationImageHandler(ParkingDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }


        public Task<string> Handle(UploadLocationImageCommand request, CancellationToken cancellationToken)
        {
            var location = _context.ParkingLocations
                                   .FirstOrDefault(pl => pl.Id == request.LocationId);
            if (location == null)
            {
                throw new Exception("Location not found");
            }
            var imagePath = _fileService.SaveImageAsync(request.File, "LocationImages").Result;
            location.ImagePath = imagePath;
            location.ImageExtension = Path.GetExtension(request.File.FileName);

            //_context.ParkingLocations.Update(location);
            _context.SaveChanges();
            return Task.FromResult(imagePath);
        }
    }
}
