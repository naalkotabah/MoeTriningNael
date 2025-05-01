using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.Entities;
using Moe.Core.Extensions;

namespace Moe.Core.Services;

public interface ICountriesService
{
    Task<Response<PagedList<CountryDTO>>> GetAll(CountryFilter filter);
    Task<Response<CountryDTO>> GetById(Guid id);
    Task<Response<CountryDTO>> Create(CountryFormDTO form);
    Task Update(CountryUpdateDTO update);
    Task Delete(Guid id);
}

public class CountriesService : BaseService, ICountriesService
{
    public CountriesService(MasterDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<Response<PagedList<CountryDTO>>> GetAll(CountryFilter filter)
    {
        var countries = await _context.Countries
            .WhereBaseFilter(filter)
            
            .Where(e => filter.Name == null || 
                        ( e.Name.En.ToLower().Contains(filter.Name))  || 
                        ( e.Name.Ar.ToLower().Contains(filter.Name)) ||
                        ( e.Name.Ku.ToLower().Contains(filter.Name)))
            
            .OrderByCreationDate()
            .ProjectTo<CountryDTO>(_mapper.ConfigurationProvider)
            .Paginate(filter);

        return new Response<PagedList<CountryDTO>>(countries, null, 200);
    }

    public async Task<Response<CountryDTO>> GetById(Guid id)
    {
        var dto = await _context.GetByIdOrException<Country, CountryDTO>(id);
        return new Response<CountryDTO>(dto, null, 200);
    }

    public async Task<Response<CountryDTO>> Create(CountryFormDTO form)
    {
        var dto = await _context.CreateWithMapper<Country, CountryDTO>(form, _mapper);
        return new Response<CountryDTO>(dto, null, 200);
    }

    public async Task Update(CountryUpdateDTO update) =>
        await _context.UpdateWithMapperOrException<Country, CountryUpdateDTO>(update, _mapper);

    public async Task Delete(Guid id)
    {
        var country = await _context.GetOrException<Country>(
            e => e.Id == id, 
            includes: q => q.Include(e => e.Cities));

        await country.Delete(_context);
        _context.Update(country);
        await _context.SaveChangesAsync();
    }
}