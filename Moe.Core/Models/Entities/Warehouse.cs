using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.Entities;

public class Warehouse : BaseEntity
{
    #region One-To-N
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    [Range(-90, 90)]
    public double? Latitude { get; set; }

    [Range(-180, 180)]
    public double? Longitude { get; set; }

    #endregion

    #region Functional
    #endregion

    #region Non-Functional
    #endregion

    #region Many-To-N
    public ICollection<User> Admins { get; set; } = new List<User>();
    #endregion
}
