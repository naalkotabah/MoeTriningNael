using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;

namespace Moe.Core.Models.Entities;

public class Country : BaseEntity
{
    #region Non-Functional
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public LocalizedContent? Name { get; set; }
    public Guid? NameId { get; set; }
    #endregion

    #region Many-To-N
    public ICollection<City> Cities { get; set; } = new List<City>();
    #endregion
    
    public override async Task Delete(MasterDbContext _context)
    {
        var cities = await _context.Cities.Where(e => e.CountryId == Id).ToListAsync();
        foreach (var city in Cities)
        {
            city.Delete(_context);
        }

        await base.Delete(_context);
    }
}