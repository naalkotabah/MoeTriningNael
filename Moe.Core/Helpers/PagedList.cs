using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Moe.Core.Helpers;

public class PagedList<T>
{
    public PagedList(List<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public List<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    //TODO-CONSIDER: Ignore deleted elements

    public static async Task<PagedList<T>> Create(IQueryable<T> query, int pageNumber, int pageSize)
    {
        pageSize = pageSize <= 15 ? pageSize : 15;

        var totalCount = query.Count();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, pageNumber, pageSize, totalCount);
    }

    public PagedList<TDto> ToDtos<TDto>(IMapper mapper)
    {
        var dtos = mapper.Map<List<T>, List<TDto>>(Items);

        return new PagedList<TDto>(dtos, PageNumber, PageSize, TotalCount);
    }
}