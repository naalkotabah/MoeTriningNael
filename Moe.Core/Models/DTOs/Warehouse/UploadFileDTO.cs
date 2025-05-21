using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.DTOs.Warehouse
{
    public class UploadFileDTO
    {
        [Required]
        public IFormFile File { get; set; }
    }

}
