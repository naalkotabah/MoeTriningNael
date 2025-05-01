using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Models.Entities;

namespace Moe.Core.Models.Statistics;

public class BaseStatisticsDTO<T> where T : BaseEntity
{
    public int? CountTotal { get; set; }
    public int? CountDeleted { get; set; }

    public int? CountCreatedToday { get; set; }
    public int? CountCreatedThisMonth { get; set; }
    public int? CountCreatedThisYear { get; set; }

    public List<int> GraphCreationByHour { get; set; }
    public List<int> GraphCreationByDay { get; set; }
    public List<int> GraphCreationByMonth { get; set; }

    public async Task AutoFill(MasterDbContext context, BaseStatisticsFilter filter = null)
    {
        CountTotal = await context.Set<T>().CountAsync();
        CountDeleted = await context.Set<T>().CountAsync(e => e.IsDeleted);

        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOfYear = new DateTime(today.Year, 1, 1);

        // Set defaults for filter values
        filter ??= new BaseStatisticsFilter();
        filter.GraphOfCreationDay ??= today.Day;
        filter.GraphOfCreationMonth ??= today.Month;
        filter.GraphOfCreationYear ??= today.Year;

        CountCreatedToday = await context.Set<T>()
            .CountAsync(e => !e.IsDeleted && e.CreatedAt.HasValue &&
                             e.CreatedAt.Value.Date == today);

        CountCreatedThisMonth = await context.Set<T>()
            .CountAsync(e => !e.IsDeleted && e.CreatedAt.HasValue &&
                             e.CreatedAt.Value.Date >= startOfMonth);

        CountCreatedThisYear = await context.Set<T>()
            .CountAsync(e => !e.IsDeleted && e.CreatedAt.HasValue &&
                             e.CreatedAt.Value.Date >= startOfYear);

        GraphCreationByHour = new List<int>(new int[24]);
        GraphCreationByDay = new List<int>(new int[30]);
        GraphCreationByMonth = new List<int>(new int[12]);

        for (int hour = 0; hour < 24; hour++)
        {
            GraphCreationByHour[hour] = await context.Set<T>()
                .CountAsync(e => !e.IsDeleted &&
                                 e.CreatedAt.HasValue &&
                                 e.CreatedAt.Value.Date == today &&
                                 e.CreatedAt.Value.Hour == hour);
        }

        var daysInMonth = DateTime.DaysInMonth(today.Year, filter.GraphOfCreationMonth.Value);
        for (int dayIndex = 1; dayIndex <= daysInMonth; dayIndex++)
        {
            var dateToCheck = new DateTime(today.Year, filter.GraphOfCreationMonth.Value, dayIndex);
            GraphCreationByDay[dayIndex - 1] = await context.Set<T>()
                .CountAsync(e => !e.IsDeleted &&
                                 e.CreatedAt.HasValue &&
                                 e.CreatedAt.Value.Date == dateToCheck);
        }

        for (int monthIndex = 1; monthIndex <= 12; monthIndex++)
        {
            GraphCreationByMonth[monthIndex - 1] = await context.Set<T>()
                .CountAsync(e => !e.IsDeleted &&
                                 e.CreatedAt.HasValue &&
                                 e.CreatedAt.Value.Year == filter.GraphOfCreationYear.Value &&
                                 e.CreatedAt.Value.Month == monthIndex);
        }
    }
}