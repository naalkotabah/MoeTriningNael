using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.DTOs;

public class WarehouseDTO : BaseDTO
{
    #region Auto
    public string Name { get; set; }
    public string? Location { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    #endregion

    #region Manual
    #endregion
} 

public class WarehouseFormDTO : BaseFormDTO
{

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string? Location { get; set; }
    [Range(-90, 90)]
    public double? Latitude { get; set; }
    [Range(-180, 180)]
    public double? Longitude { get; set; }
}

public class WarehouseUpdateDTO : BaseUpdateDTO
{

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }


    [Range(-90, 90)]
    public double? Latitude { get; set; }

    [Range(-180, 180)]
    public double? Longitude { get; set; }
}

public class WarehouseFilter : BaseFilter
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }
}
