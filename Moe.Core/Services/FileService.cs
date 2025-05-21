namespace Moe.Core.Services
{
    public interface IFileService
    {
        Task<string> UploadSingleAsync(IFormFile file);
        Task<List<string>> UploadMultipleAsync(List<IFormFile> files);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadSingleAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("الملف غير صالح");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "images");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/images/{uniqueName}";
        }

        public async Task<List<string>> UploadMultipleAsync(List<IFormFile> files)
        {
            if (files == null || !files.Any())
                throw new ArgumentException("الملفات غير موجودة");

            var result = new List<string>();

            foreach (var file in files)
            {
                var path = await UploadSingleAsync(file); // إعادة استخدام الدالة
                result.Add(path);
            }

            return result;
        }
    }

}
