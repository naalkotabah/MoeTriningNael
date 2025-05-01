namespace Moe.Core.Models.DTOs;

public class PermissionDTO : BaseDTO
{
    #region Auto
    public string? Subject { get; set; }
    public string? Action { get; set; }
    public string? FullName { get; set; }
    #endregion

    #region Manual
    #endregion
} 

public class PermissionFilter : BaseFilter
{
    public string? Subject { get; set; }
    public string? Action { get; set; }
    public string? FullName { get; set; }
}