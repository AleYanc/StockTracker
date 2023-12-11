
using Microsoft.AspNetCore.Hosting;

namespace StockTracker.Services
{
    public class LocalFileSaver(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : IFileSaver
    {
        private readonly IWebHostEnvironment env = env;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

        public Task DeleteFile(string container, string route)
        {
            if (route != null)
            {
                string fileName = Path.GetFileName(route);
                string directory = Path.Combine(env.WebRootPath, container, fileName);

                if (File.Exists(directory))
                {
                    File.Delete(directory);
                }
            }
            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string contentType, string route)
        {
            await DeleteFile(container, route);
            return await SaveFile(content, extension, container, contentType);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string contentType)
        {
            string fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, container);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string route = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(route, content);

            string actualUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            string dbUrl = Path.Combine(actualUrl, container, fileName).Replace("\\", "/");
            return dbUrl;
        }

        public async Task<string> LocalFileSave(IFormFile file, IWebHostEnvironment webHostEnvironment)
        {
            if (file.Length == 0)
            {
                throw new BadHttpRequestException("File is empty.");
            }

            string extension = Path.GetExtension(file.FileName);

            string webRootPath = webHostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            string folderPath = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = $"{Guid.NewGuid()}.{extension}";
            string filePath = Path.Combine(folderPath, fileName);
            using FileStream stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return filePath;
        }
    }
}
