using System.ComponentModel.DataAnnotations;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Models.DTOs;

namespace Moe.Core.Models.Entities;

public class SystemSettings : BaseEntity
{
    #region One-To-N
    #endregion
    
    #region Functional
    #endregion
    
    #region Non-Functional
    #endregion
    
    #region Many-To-N
    #endregion

    #region Helper methods

    public static async Task<SystemSettings> Get(MasterDbContext _dbContext, IMapper _mapper)
    {
        var entity = await _dbContext.SystemSettings
            .ProjectTo<SystemSettings>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        if (entity == null)
        {
            var newSysSettings = new SystemSettings();
            await _dbContext.AddAsync(newSysSettings);
            await _dbContext.SaveChangesAsync();

            entity = newSysSettings;
        }

        return entity;
    }
    public static async Task<SystemSettingsDTO> GetDto(MasterDbContext _dbContext, IMapper _mapper)
    {
        var dto = await _dbContext.SystemSettings
            .ProjectTo<SystemSettingsDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        if (dto == null)
        {
            var newSysSettings = new SystemSettings();
            await _dbContext.AddAsync(newSysSettings);
            await _dbContext.SaveChangesAsync();
            
            dto = await _dbContext.SystemSettings
                .ProjectTo<SystemSettingsDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        return dto;
    }

    #endregion
}
