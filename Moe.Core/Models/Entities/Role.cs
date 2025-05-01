using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.Entities;

public class Role : BaseEntity
{
    #region Non-Functional
    [MaxLength(128)] 
    public string Name { get; set; }
    #endregion

    #region Many-To-N
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    #endregion
}
