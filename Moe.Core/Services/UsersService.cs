using AutoMapper;
using AutoMapper.QueryableExtensions;
using Moe.Core.Data;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.User;
using Moe.Core.Extensions;
using Moe.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Null;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Moe.Core.Services;

public interface IUsersService
{
    Task<Response<PagedList<UserDTO>>> GetAll(UserFilterDTO filter);
    Task<Response<UserDTO>> GetById(Guid id);
    Task<Response<UserDTO>> Create(UserFormDTO form);
    Task Update(Guid id, UserUpdateDTO update);
    Task<Response<string>> Delete(Guid id, bool isPermanent);

    Task<Response<string>> SetUserState(Guid id);

}

public class UsersService : BaseService, IUsersService
{
    public UsersService(MasterDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
    public async Task<Response<PagedList<UserDTO>>> GetAll(UserFilterDTO filter)
    {
        var query = _context.Users
            .WhereBaseFilter(filter)
            .Where(e => filter.Role == null || e.StaticRole == filter.Role)
            .Where(e => filter.Email == null || e.Email.ToLower().Contains(filter.Email))
            .Where(e => filter.Name == null || e.Name.ToLower().Contains(filter.Name))
            .Where(e => filter.Phone == null || (e.Phone + e.PhoneCountryCode).ToLower().Contains(filter.Phone));




        var users = await query
            .OrderByCreationDate()
            .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
            .Paginate(filter);

        return new Response<PagedList<UserDTO>>(users, null, 200);
    }
    public async Task<Response<UserDTO>> GetById(Guid id)
    {
        var dto = await _context.GetByIdOrException<User, UserDTO>(id);
        return new Response<UserDTO>(dto, null, 200);
    }
    public async Task<Response<UserDTO>> Create(UserFormDTO form)
    {

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == form.Email || u.Phone == form.Phone);

        if (existingUser != null)
        {
            return new Response<UserDTO>(null, "Email or Phone already exists.", 400);
        }


        var user = _mapper.Map<User>(form);

   
        var (passwordHash, passwordSalt) = PasswordHelper.CreatePasswordHash(form.Password);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

   
        user.StaticRole = form.StaticRole; 

       
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

      
        var userDTO = _mapper.Map<UserDTO>(user);

      
        return new Response<UserDTO>(userDTO, null, 201);  
    }

    public async Task Update(Guid id, UserUpdateDTO update)
    {
     
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            ErrResponseThrower.NotFound("User not found.");
        }

      
        if (id != update.Id)
        {
            
            user.StaticRole = update.StaticRole;
        }

      
        user.Email = update.Email ?? user.Email;
        user.Phone = update.Phone ?? user.Phone;
        user.PhoneCountryCode = update.PhoneCountryCode ?? user.PhoneCountryCode;
        user.Name = update.Name ?? user.Name;

      
        if (!string.IsNullOrEmpty(update.Password))
        {
            var (passwordHash, passwordSalt) = PasswordHelper.CreatePasswordHash(update.Password);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

      
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<Response<string>> Delete(Guid id, bool isPermanent)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return new Response<string>(null, "User not found.", 400);
        }

        if (isPermanent)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new Response<string>(null, "User permanently deleted.", 200);
        }
        else
        {
            user.IsDeleted = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new Response<string>(null, "User soft-deleted (marked as deleted).", 200);
        }
    }
    public async Task<Response<string>> SetUserState(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return new Response<string>(null, "User not found", 404);

        user.IsBanned = user.IsBanned == UserState.Active ? UserState.Band : UserState.Active;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        var message = user.IsBanned == UserState.Band ? "User state set to banned." : "User state set to active.";
        return new Response<string>(message, null, 200);
    }


}