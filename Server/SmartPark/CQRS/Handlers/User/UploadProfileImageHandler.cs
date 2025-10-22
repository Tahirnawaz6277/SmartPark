using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartPark.CQRS.Commands.User;
using SmartPark.Data.Contexts;
using SmartPark.Services.Interfaces;

namespace SmartPark.CQRS.Handlers.User
{
    public class UploadProfileImageHandler : IRequestHandler<UploadProfileImageCommand, string>
    {
        private readonly ParkingDbContext _context;
        private readonly IFileService _fileService;

        public UploadProfileImageHandler(ParkingDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<string> Handle(UploadProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (user == null)
                throw new Exception("User not found");

            var path = await _fileService.SaveImageAsync(request.File, "Profile");
            user.ProfileImagePath = path;
            user.ImageExtension = Path.GetExtension(request.File.FileName);

            await _context.SaveChangesAsync(cancellationToken);
            return path;
        }
    }
}
