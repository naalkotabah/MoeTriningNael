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
    Task<Response<PagedList<UserDTO>>> GetAll(UserFilterDTO filter , string currentRole);
    Task<Response<UserDTO>> GetById(Guid id);
    Task<Response<UserDTO>> Create(UserFormDTO form);
    Task Update(Guid id, UserUpdateDTO update);
    Task Delete(Guid id, bool isPermanent);

    Task<Response<string>> SetUserState(SetUserStateDTO dto);

}

public class UsersService : BaseService, IUsersService
{
    public UsersService(MasterDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
    public async Task<Response<PagedList<UserDTO>>> GetAll(UserFilterDTO filter, string currentRole)
    {
        var query = _context.Users
            .WhereBaseFilter(filter)
            .Where(e => filter.Role == null || e.StaticRole == filter.Role)
            .Where(e => filter.Email == null || e.Email.ToLower().Contains(filter.Email))
            .Where(e => filter.Name == null || e.Name.ToLower().Contains(filter.Name))
            .Where(e => filter.Phone == null || (e.Phone + e.PhoneCountryCode).ToLower().Contains(filter.Phone));


        if (Enum.TryParse<StaticRole>(currentRole, true, out var roleEnum) && roleEnum == StaticRole.NORMAL)
        {
            query = query.Where(e => e.StaticRole == StaticRole.NORMAL);
        }


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

    public async Task Delete(Guid id, bool isPermanent)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            throw new ArgumentException("User not found.");
        }

        if (isPermanent)
        {       
            _context.Users.Remove(user);
        }
        else
        {
           
            user.IsDeleted = true;
            _context.Users.Update(user);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Response<string>> SetUserState(SetUserStateDTO dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null)
            return new Response<string>(null, "User not found", 404);

        if (user.IsBanned == dto.NewState)
            return new Response<string>(null, $"User is already {dto.NewState}.", 400);

        user.IsBanned = dto.NewState;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return new Response<string>($"User state set to {dto.NewState}.", null, 200);
    }
}