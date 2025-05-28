using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Moe.Core.Models.DTOs;

public class ItemDTO : BaseDTO
{
    #region Auto
    public string Name { get; set; }
    public string Details { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }

    public Guid CreatedByUserId { get; set; }
    #endregion

    #region Manual
    #endregion
}
public class ItemFormDTO : BaseFormDTO
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [Required]
    [MaxLength(500)]
    public string Details { get; set; }
    public string? ImageUrl { get; set; }
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
  [JsonIgnore] public Guid CreatedByUserId { get; set; }
}
public class ItemUpdateDTO : BaseUpdateDTO
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Details { get; set; }
    public string? ImageUrl { get; set; }
    [Required]
    public decimal Price { get; set; }
 
    [Required]
    [JsonIgnore] public Guid CreatedByUserId { get; set; }
}

public class ItemFilter : BaseFilter
{
    public string Name { get; set; }
    public string Details { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public Guid CreatedByUserId { get; set; }
}
