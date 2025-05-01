using Moe.Core.Null;

namespace Moe.Core.Services;

public interface IAttachmentsService
{
    Task<string> Upload(IFormFile file);
}

public class AttachmentsService : IAttachmentsService
{
    public async Task<string> Upload(IFormFile file)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine("wwwroot", uniqueFileName);

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        catch
        {
            ErrResponseThrower.InternalServerErr("UPLOAD_FILE_FAILED");
        }

        return uniqueFileName;
    }
}