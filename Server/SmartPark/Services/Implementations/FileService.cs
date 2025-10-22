using SmartPark.Services.Interfaces;

namespace SmartPark.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;
        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            var uploadsFolder = Path.Combine(_env.WebRootPath,"Uploads", folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyToAsync(fileStream);
            }
            // Return relative path to store in DB
            return Task.FromResult(Path.Combine("Uploads", folder, uniqueFileName).Replace("\\", "/"));
        }
    }
}
