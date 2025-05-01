using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.Entities;
using Moe.Core.Null;

namespace Moe.Core.Data;

public static class MasterDbContextCrudCreateExtensions
{
    public static async Task<TDto> CreateWithMapper<TEntity,TDto>(
        this MasterDbContext context,
        object form,
        IMapper mapper
    )
        where TEntity : BaseEntity
    {
        var entity = mapper.Map<TEntity>(form);
        await context.Set<TEntity>().AddAsync(entity);
        await context.SaveChangesAsync();

        var dto = await context.Set<TEntity>()
            .Where(e => e.Id == entity.Id)
            .ProjectTo<TDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
        return dto;
    }
}

public static class MasterDbContextCrudReadExtensions
{
     public static async Task<T> GetOrException<T>(
         this MasterDbContext context,
         Expression<Func<T, bool>> predicate,
         string errMsgKey = null)
         where T : BaseEntity
     {
         return await context.GetOrException(predicate, null, errMsgKey);
     }
 
     public static async Task<T> GetOrException<T>(
         this MasterDbContext context,
         Expression<Func<T, bool>> predicate,
         Expression<Func<IQueryable<T>, IQueryable<T>>> includes,
         string errMsgKey = null)
         where T : BaseEntity
     {
         var query = context.Set<T>().Where(predicate).Where(e => !e.IsDeleted);
         
         if (includes != null)
             query = includes.Compile()(query);
 
         var entity = await query.FirstOrDefaultAsync();
         if (entity == null)
             ErrResponseThrower.NotFound(errMsgKey);
 
         return entity;
     }
 
     public static async Task<TDto> GetOrException<T, TDto>(
         this MasterDbContext context,
         Expression<Func<T, bool>> predicate,
         string errMsgKey = null)
         where T : BaseEntity
         where TDto : BaseDTO
     {
         var dto = await context.Set<T>()
             .Where(predicate)
             .Where(e => !e.IsDeleted)
             .ProjectTo<TDto>(context._mapper.ConfigurationProvider)
             .FirstOrDefaultAsync();
 
         if (dto == null)
             ErrResponseThrower.NotFound(errMsgKey);
 
         return dto;
     }
     
     public static async Task<T> GetByIdOrException<T>(
         this MasterDbContext context,
         Guid id,
         string errMsgKey = null)
         where T : BaseEntity
     {
         return await context.GetByIdOrException<T>(id, null, errMsgKey);
     }
 
     public static async Task<T> GetByIdOrException<T>(
         this MasterDbContext context,
         Guid id,
         Expression<Func<IQueryable<T>, IQueryable<T>>> includes,
         string errMsgKey = null)
         where T : BaseEntity
     {
         var query = context.Set<T>().Where(e => e.Id == id && !e.IsDeleted);
 
         if (includes != null)
         {
             query = includes.Compile()(query);
         }
 
         var entity = await query.FirstOrDefaultAsync();
         if (entity == null)
             ErrResponseThrower.NotFound(errMsgKey);
 
         return entity;
     }
 
     public static async Task<TDto> GetByIdOrException<T, TDto>(
         this MasterDbContext context,
         Guid id,
         string errMsgKey = null)
         where T : BaseEntity
         where TDto : BaseDTO
     {
         var dto = await context.Set<T>()
             .Where(e => e.Id == id && !e.IsDeleted)
             .ProjectTo<TDto>(context._mapper.ConfigurationProvider)
             .FirstOrDefaultAsync();
 
         if (dto == null)
             ErrResponseThrower.NotFound(errMsgKey);
 
         return dto;
     }
     
     
     public static async Task<T> GetById<T>(
         this MasterDbContext context,
         Guid id) 
         where T : BaseEntity
     {
         return await context.GetById<T>(id, null);
     }
 
     public static async Task<T> GetById<T>(
         this MasterDbContext context,
         Guid id,
         Expression<Func<IQueryable<T>, IQueryable<T>>> includes)
         where T : BaseEntity
     {
         var query = context.Set<T>().Where(e => e.Id == id && !e.IsDeleted);
 
         if (includes != null)
             query = includes.Compile()(query);
 
         var entity = await query.FirstOrDefaultAsync();
         return entity;
     }
     
     public static async Task<TDto> GetById<T,TDto>(
         this MasterDbContext context,
         Guid id)
         where T : BaseEntity
     {
         var dto = await context.Set<T>()
             .Where(e => e.Id == id)
             .ProjectTo<TDto>(context._mapper.ConfigurationProvider)
             .FirstOrDefaultAsync();
         return dto;
     }   
}

public static class MasterDbContextCrudUpdateExtensions
{
    public static async Task UpdateWithMapperOrException<TEntity, TUpdate>(
        this MasterDbContext context,
        TUpdate update,
        IMapper mapper)
        where TEntity : BaseEntity
        where TUpdate : BaseUpdateDTO
    {
        var entity = await context.Set<TEntity>().FindAsync(update.Id);
        if (entity == null)
            ErrResponseThrower.NotFound();
        mapper.Map(update, entity);
        context.Update(entity);
        await context.SaveChangesAsync();
    }
}

public static class MasterDbContextCrudDeleteExtensions
{
    public static async Task SoftDeleteOrException<T>(
        this MasterDbContext context,
        Guid id,
        string? errMsgEn = null,
        string? errMsgAr = null,
        string? errMsgKu = null)
        where T : BaseEntity
    {
        var entity = await context.Set<T>()
            .Where(e => e.Id == id && !e.IsDeleted)
            .FirstOrDefaultAsync();
        if (entity == null)
            ErrResponseThrower.NotFound();

        await entity.Delete(context);
        context.Update(entity);
        await context.SaveChangesAsync();
    }
}