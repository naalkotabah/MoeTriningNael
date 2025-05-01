using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.Entities;

namespace Moe.Core.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> WhereBaseFilter<T>(this IQueryable<T> query, BaseFilter filter) where T : BaseEntity
    {
        return query
            .Where(e => filter.Id == null || e.Id == filter.Id)
            .WhereSoftDeleted(filter.IsDeleted)
            .WhereDateRange(filter.StartDate, filter.EndDate);
    }
    
    public static IQueryable<T> WhereSoftDeleted<T>(this IQueryable<T> query, bool? filterValue) where T : BaseEntity
    {
        if (filterValue == null)
            return query;

        return query.Where(e => e.IsDeleted == filterValue);
    }
    
    public static IQueryable<T> WhereDateRange<T>(
        this IQueryable<T> query,
        DateTime? startDate,
        DateTime? endDate) where T : BaseEntity
    {
        return query
            .Where(e => startDate == null || e.CreatedAt >= startDate)
            .Where(e => endDate == null || e.CreatedAt <= endDate);
    }

    public static IQueryable<T> OrderByCreationDate<T>(this IQueryable<T> query) where T : BaseEntity
    {
        return query.OrderByDescending(e => e.CreatedAt);
    }
}