using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moe.Core.Models.Entities;

namespace Moe.Core.Data.Interceptors;

public class DateSetterInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null) return base.SavingChangesAsync(eventData, result, cancellationToken);
        
        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {

            if (entry is { State: EntityState.Added, Entity: BaseEntity createdEntity })
            {
                createdEntity.CreatedAt = DateTime.UtcNow;
                createdEntity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry is { State: EntityState.Modified, Entity: BaseEntity updatedEntity })
            {
                updatedEntity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}