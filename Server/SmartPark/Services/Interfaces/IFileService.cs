namespace SmartPark.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile file, string folder);

    }
}
