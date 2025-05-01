using AutoMapper;
using Moe.Core.Models.DTOs;
using Moe.Core.Data;
using Moe.Core.Helpers;
using Moe.Core.Models.Entities;

namespace Moe.Core.Services;

public interface ISystemSettingsService
{
    Task<Response<SystemSettingsDTO>> Get();
    Task Update(SystemSettingsUpdateDTO update);
}

public class SystemSettingsService : BaseService, ISystemSettingsService
{
    public SystemSettingsService(MasterDbContext context, IMapper mapper): base(context, mapper)
    {}

    public async Task<Response<SystemSettingsDTO>> Get()
    {
        var dto = await SystemSettings.GetDto(_context, _mapper);
        return new Response<SystemSettingsDTO>(dto, null, 200);
    }
    public async Task Update(SystemSettingsUpdateDTO update)
    {
        var entity = await SystemSettings.Get(_context, _mapper);
        _mapper.Map(update,entity);
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }
}