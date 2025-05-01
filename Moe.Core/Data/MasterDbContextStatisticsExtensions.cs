using Microsoft.EntityFrameworkCore;
using Moe.Core.Models.Entities;

namespace Moe.Core.Data;

public static class MasterDbContextStatisticsExtensions
{
    public static async Task<List<int>> GetGraphCreationHourly<T>(this MasterDbContext context)
            where T : BaseEntity
        {
            var now = DateTime.UtcNow;
            var startTime = now.AddHours(-24);
            return await context.Set<T>()
                .Where(e => e.CreatedAt >= startTime && e.CreatedAt <= now && !e.IsDeleted)
                .GroupBy(e => new { e.CreatedAt.Value.Hour })
                .Select(g => g.Count())
                .ToListAsync();
        }

        public static async Task<List<int>> GetGraphCreationDaily<T>(this MasterDbContext context)
            where T : BaseEntity
        {
            var now = DateTime.UtcNow;
            var startTime = now.AddDays(-30);
            return await context.Set<T>()
                .Where(e => e.CreatedAt >= startTime && e.CreatedAt <= now && !e.IsDeleted)
                .GroupBy(e => new { e.CreatedAt.Value.Date })
                .Select(g => g.Count())
                .ToListAsync();
        }

        public static async Task<List<int>> GetGraphCreationMonthly<T>(this MasterDbContext context)
            where T : BaseEntity
        {
            var now = DateTime.UtcNow;
            var startTime = now.AddYears(-1);
            return await context.Set<T>()
                .Where(e => e.CreatedAt >= startTime && e.CreatedAt <= now && !e.IsDeleted)
                .GroupBy(e => new { Year = e.CreatedAt.Value.Year, Month = e.CreatedAt.Value.Month })
                .Select(g => g.Count())
                .ToListAsync();
        }

        public static async Task<List<int>> GetGraphCreationYearly<T>(this MasterDbContext context)
            where T : BaseEntity
        {
            var now = DateTime.UtcNow;
            var startTime = now.AddYears(-5);
            return await context.Set<T>()
                .Where(e => e.CreatedAt >= startTime && e.CreatedAt <= now && !e.IsDeleted)
                .GroupBy(e => e.CreatedAt.Value.Year)
                .Select(g => g.Count())
                .ToListAsync();
        }    
}