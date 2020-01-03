using DotnetBoilerplate.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using System.Threading.Tasks;

namespace DotnetBoilerplate.Helpers
{
    public class FileProvider : ISelfScopedLifetime
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileProvider(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public string WebRootPath
        {
            get
            {
                return _hostingEnvironment.WebRootPath;
            }
        }

        public string ContentRootPath
        {
            get
            {
                return _hostingEnvironment.ContentRootPath;
            }
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public async Task SaveFile(string path, IFormFile file)
        {
            using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, true);
            await file.CopyToAsync(stream);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            provider.TryGetContentType(path, out string contentType);

            return contentType;
        }

        public string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public byte[] GetFileBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}