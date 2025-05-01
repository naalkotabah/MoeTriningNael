using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.Entities;
using Moe.Core.Extensions;
using Moe.Core.Null;

namespace Moe.Core.Services;

public interface ICitiesService
{
    Task<Response<PagedList<CityDTO>>> GetAll(CityFilter filter);
    Task<Response<CityDTO>> GetById(Guid id);
    Task<Response<CityDTO>> Create(CityFormDTO form);
    Task Update(CityUpdateDTO update);
    Task Delete(Guid id);
}

public class CitiesService : BaseService, ICitiesService
{
    public CitiesService(MasterDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<Response<PagedList<CityDTO>>> GetAll(CityFilter filter)
    {
        var cities = await _context.Cities
            .WhereBaseFilter(filter)
            
            .Where(e => filter.CountryId == null || filter.CountryId == e.CountryId)
            .Where(e => filter.CountryName == null || 
                        ( e.Country.Name.En.ToLower().Contains(filter.CountryName) ) ||
                        ( e.Country.Name.Ar.ToLower().Contains(filter.CountryName) ))
            .Where(e => filter.Name == null || 
                        ( e.Name.En.ToLower().Contains(filter.Name))  || 
                        ( e.Name.Ar.ToLower().Contains(filter.Name)) ||
                        ( e.Name.Ku.ToLower().Contains(filter.Name)))
            
            .OrderByCreationDate()
            .ProjectTo<CityDTO>(_mapper.ConfigurationProvider)
            .Paginate(filter);

        return new Response<PagedList<CityDTO>>(cities, null, 200);
    }

    public async Task<Response<CityDTO>> GetById(Guid id)
    {
        var dto = await _context.GetByIdOrException<City, CityDTO>(id);
        return new Response<CityDTO>(dto, null, 200);
    }

    public async Task<Response<CityDTO>> Create(CityFormDTO form)
    {
        var countryExists = await _context.Countries.AnyAsync(e => e.Id == form.CountryId);
        if (!countryExists)
            ErrResponseThrower.NotFound("COUNTRY_NOT_FOUND");

        var dto = await _context.CreateWithMapper<City, CityDTO>(form, _mapper);
        return new Response<CityDTO>(dto, null, 200);
    }

    public async Task Update(CityUpdateDTO update)
    {
        if (update.CountryId != null)
        {
            var countryExists = await _context.Countries.AnyAsync(e => e.Id == update.CountryId);
            if (!countryExists)
                ErrResponseThrower.NotFound("COUNTRY_NOT_FOUND");
        }

        await _context.UpdateWithMapperOrException<City, CityUpdateDTO>(update, _mapper);
    }

    public async Task Delete(Guid id)
    {
        var city = await _context.GetOrException<City>(e => e.Id == id);

        await city.Delete(_context);
        _context.Update(city);
        await _context.SaveChangesAsync();
    }
}