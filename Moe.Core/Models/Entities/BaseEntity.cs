using System.ComponentModel.DataAnnotations;
using Moe.Core.Data;

namespace Moe.Core.Models.Entities;

public class BaseEntity
{
    [Key] public Guid Id { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
 

    public virtual async Task Delete(MasterDbContext _context)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
    public virtual async Task UnDelete(MasterDbContext _context)
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}