using Microsoft.EntityFrameworkCore;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;

namespace Moe.Core.Extensions;

public static class DbSetExtensions
{
    public static async Task<PagedList<T>> Paginate<T>(this IQueryable<T> query, BaseFilter filter)
    {
        if (query == null)
            return null;

        return await PagedList<T>.Create(query, filter.PageNumber, filter.PageSize);
    }
}