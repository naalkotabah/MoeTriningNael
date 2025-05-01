using Microsoft.EntityFrameworkCore;
using Moe.Core.Models.Entities;
using Moe.Core.Null;

namespace Moe.Core.Data;

public static class MasterDbContextValidationExtensions
{
    public static async Task EnsureEntityExists<T>(this MasterDbContext context,
        Guid? id,
        string errMsgKey = null)
    where T : BaseEntity
    {
        if (id == null) return;
        var entityExists = await context.Set<T>().AnyAsync(e => e.Id == id && !e.IsDeleted);
        if (!entityExists)
            ErrResponseThrower.NotFound(errMsgKey);
    }

    public static async Task EnsureEntitiesIdsExists<T>(this MasterDbContext context,
        List<Guid>? ids = null,
        string errMsgKey = null)
        where T : BaseEntity
    {
        foreach (var id in ids ?? Enumerable.Empty<Guid>())
        {
            var isValid = await context.Set<T>().AnyAsync(e => e.Id == id && !e.IsDeleted);
            if (!isValid)
                ErrResponseThrower.NotFound(errMsgKey);
        }
    }
}
