namespace StockTracker.Services
{
    public interface IFileSaver
    {
        Task<string> SaveFile(byte[] content, string extension, 
            string container, string contentType);
        Task<string> EditFile(byte[] content, string extension, 
            string container, string contentType, string route);
        Task<string> LocalFileSave(IFormFile file, IWebHostEnvironment webHostEnvironment);
        Task DeleteFile(string container, string route);
    }
}
