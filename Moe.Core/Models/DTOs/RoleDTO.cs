using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.DTOs;

public class RoleDTO : BaseDTO
{
    #region Auto
    public string? Name { get; set; }
    #endregion

    #region Manual
    #endregion
} 

public class RoleFormDTO : BaseFormDTO
{
    [Required]
    [StringLength(128, MinimumLength = 2)]
    public string Name { get; set; }
}

public class RoleUpdateDTO : BaseUpdateDTO
{
    [StringLength(128, MinimumLength = 2)]
    public string? Name { get; set; }
}

public class RoleFilter : BaseFilter
{
    public string? Name { get; set; }
}
