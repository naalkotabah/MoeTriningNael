
using System.ComponentModel.DataAnnotations;
using Moe.Core.Models.DTOs.LocalizedContent;

namespace Moe.Core.Models.DTOs;

public class CityDTO : BaseDTO
{
    #region Auto
    public Guid? CountryId { get; set; }
    public LocalizedContentDTO? CountryName { get; set; }

    public LocalizedContentDTO? Name { get; set; }
    #endregion

    #region Manual
    #endregion
}

public class CityFormDTO : BaseFormDTO
{
    [Required] public Guid CountryId { get; set; }

    [Required]
    public LocalizedContentFormDTO Name { get; set; }
}

public class CityUpdateDTO : BaseUpdateDTO
{
    public Guid? CountryId { get; set; }
    public LocalizedContentUpdateDTO? Name { get; set; }
}

public class CityFilter : BaseFilter
{
    public Guid? CountryId { get; set; }
    public string? CountryName { get; set; }
    public string? Name { get; set; }
}
