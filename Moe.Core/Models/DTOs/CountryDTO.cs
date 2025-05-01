
using System.ComponentModel.DataAnnotations;
using Moe.Core.Models.DTOs.LocalizedContent;

namespace Moe.Core.Models.DTOs;

public class CountryDTO : BaseDTO
{
    #region Auto
    public LocalizedContentDTO? Name { get; set; }
    #endregion

    #region Manual
    #endregion
}

public class CountryFormDTO : BaseFormDTO
{
    [Required]
    public LocalizedContentFormDTO Name { get; set; }
}

public class CountryUpdateDTO : BaseUpdateDTO
{
    public LocalizedContentUpdateDTO? Name { get; set; }
}

public class CountryFilter : BaseFilter
{
    public string? Name { get; set; }
}
