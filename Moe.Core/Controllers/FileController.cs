using Microsoft.AspNetCore.Mvc;
using Moe.Core.Controllers;
using Moe.Core.Models.DTOs.Warehouse;
using Moe.Core.Services;


public class FilesController : BaseController
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadSingle([FromForm] UploadFileDTO dto)
    {
        try
        {
            var path = await _fileService.UploadSingleAsync(dto.File);
            return Ok(new { path });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultiple([FromForm] List<IFormFile> files)
    {
        try
        {
            var paths = await _fileService.UploadMultipleAsync(files);
            return Ok(paths);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
