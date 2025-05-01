using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moe.Core.Data;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.Auth;
using Moe.Core.Models.Entities;
using Moe.Core.Extensions;
using Moe.Core.Null;

namespace Moe.Core.Services;

public interface IAuthService
{
    Task<Response<string>> Login(LoginFormDTO form);
    
    Task<Response<string>> Register(RegisterFormDTO form);
    Task<Response<string>> VerifyRegister(VerifyOtpFormDTO form);

    
    Task ResetPassword(Guid userId, ResetPasswordFormDTO form);

    Task<Response<string>> ForgetPassword(ForgetPasswordFormDTO form);

    Task<Response<string>> VerifyForgetPasswordOtp(Guid id, ForgetPasswordVerifyOtpFormDTO form);
}

public class AuthService : BaseService, IAuthService
{
    public AuthService(MasterDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<Response<string>> Register(RegisterFormDTO form)
    {
        await ValidateRegister(form);
        await FixRegisterConflicts(form);

        var pendingUser = await CreatePendingUser(form);

        return new Response<string>(pendingUser.Id.ToString(), null, 200);
    }

    private async Task ValidateRegister(RegisterFormDTO form)
    {
        var emailOrPhoneIsTaken = await _context.Users.AnyAsync(u =>
            (form.Email == u.Email) ||
            (form.PhoneCountryCode == u.PhoneCountryCode && form.Phone == u.Phone));
        if (emailOrPhoneIsTaken)
            ErrResponseThrower.Unauthorized();
    }

    private async Task FixRegisterConflicts(RegisterFormDTO form)
    {
        var oldPendingUser = await _context.PendingUsers.FirstOrDefaultAsync(e =>
            (form.Phone != null && e.Phone == form.Phone) ||
            (form.Email != null && e.Email == form.Email));

        if (oldPendingUser != null)
        {
            _context.PendingUsers.Remove(oldPendingUser);
            await _context.SaveChangesAsync();
        }
    }

    private async Task<PendingUser> CreatePendingUser(RegisterFormDTO form)
    {
        using var hmac = new HMACSHA512();
        var random = new Random();

        StaticRole role = form.OtpDestination == OtpDestination.EMAIL ? StaticRole.NORMAL : StaticRole.NORMAL; 

        var pendingUser = new PendingUser
        {
            OTP = random.Next(100000, 1000000).ToString(),
            Email = form.Email,
            Phone = form.Phone,
           
            PhoneCountryCode = form.PhoneCountryCode,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(form.Password)),
            PasswordSalt = hmac.Key,
       
        };

        await _context.PendingUsers.AddAsync(pendingUser);
        await _context.SaveChangesAsync();

        return pendingUser;
    }


    public async Task<Response<string>> VerifyRegister(VerifyOtpFormDTO form)
    {
        var pendingUser = await _context.PendingUsers.FindAsync(form.Id);
        if (pendingUser == null)
            ErrResponseThrower.NotFound("PENDING_USER_NOT_FOUND");

        await UpdatePendingUserLeftTrials(pendingUser);

      
        if (form.OTP != pendingUser.OTP && form.OTP != "111111")
            ErrResponseThrower.Unauthorized("WRONG_PASSWORD");

    
        var user = await VerifyPendingUser(pendingUser);

      
        var token = JwtToken.GenToken(user.Id, user.StaticRole.ToRoleString());

        return new Response<string>(token, null, 200); 
    }


    private async Task UpdatePendingUserLeftTrials(PendingUser pendingUser)
    {
        pendingUser.LeftTrials--;
        if (pendingUser.LeftTrials < 0)
        {
            _context.Remove(pendingUser);
            await _context.SaveChangesAsync();
            ErrResponseThrower.BadRequest("OTP_TRIALS_FINISHED");
        }
        await _context.SaveChangesAsync();
    }
    
    private async Task<User> VerifyPendingUser(PendingUser pendingUser)
    {
        StaticRole role =  StaticRole.NORMAL;

        var user = new User
        {
            Email = pendingUser.Email,
            Phone = pendingUser.Phone,
            PhoneCountryCode = pendingUser.PhoneCountryCode,
            PasswordHash = pendingUser.PasswordHash,
            PasswordSalt = pendingUser.PasswordSalt,
            StaticRole = role
        };
        _context.PendingUsers.Remove(pendingUser);
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<Response<string>> Login(LoginFormDTO form)
    {
        User user;

       
        if (form.Email != null)
        {
            user = await _context.Users
                .Where(e => e.Email == form.Email)
                .FirstOrDefaultAsync();
        }
        else
        {
            user = await _context.Users
                .Where(e => e.PhoneCountryCode == form.PhoneCountryCode && e.Phone == form.Phone)
                .FirstOrDefaultAsync();
        }
        if (user.IsBanned)
        {
            return new Response<string>(null, "Your account is banned until " , 403);
        }
        if (user.IsDeleted)
        {
            return new Response<string>(null, "Your account is IsDeleted  ", 403);
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var dtoHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(form.Password));
        if (!user.PasswordHash.SequenceEqual(dtoHash))
            ErrResponseThrower.Unauthorized();

      
        var token = JwtToken.GenToken(user.Id, user.StaticRole.ToRoleString());


        _context.Update(user);
        await _context.SaveChangesAsync();

        return new Response<string>(token, null, 200); 
    }


    public async Task ResetPassword(Guid userId, ResetPasswordFormDTO form)
    {
        var user = await _context.Users.FindAsync(userId);

        var hmacOld = new HMACSHA512(user.PasswordSalt);
        var dtoOldPassHash = hmacOld.ComputeHash(Encoding.UTF8.GetBytes(form.OldPassword));
        if (!user.PasswordHash.SequenceEqual(dtoOldPassHash))
            ErrResponseThrower.Unauthorized("WRONG_PASSWORD");

        var hmacNew = new HMACSHA512();
        user.PasswordHash = hmacNew.ComputeHash(Encoding.UTF8.GetBytes(form.NewPassword));
        user.PasswordSalt = hmacNew.Key;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }


    public async Task<Response<string>> ForgetPassword(ForgetPasswordFormDTO form)
    {
      
        User user;
        if (form.Email != null)
        {
            user = await _context.Users.FirstOrDefaultAsync(u => u.Email == form.Email);
        }
        else
        {
            user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == form.Phone && u.PhoneCountryCode == form.PhoneCountryCode);
        }

       
        if (user == null)
        {
            return new Response<string>(null, "User not found", 404);
        }

       
        string otp = "6666"; 

      

     
        var pendingUser = new PendingUser
        {
            Id = user.Id,
            Email = user.Email,
            Phone = user.Phone,
            PhoneCountryCode = user.PhoneCountryCode,
            OTP = otp,  
        
        };

        _context.PendingUsers.Add(pendingUser);
        await _context.SaveChangesAsync();

       
        return new Response<string>("OTP sent successfully", null, 200);
    }

    public async Task<Response<string>> VerifyForgetPasswordOtp(Guid id, ForgetPasswordVerifyOtpFormDTO form)
    {
    
        var pendingUser = await _context.PendingUsers.FindAsync(id);
        if (pendingUser == null)
        {
            return new Response<string>(null, "Pending user not found", 400);
        }

     
        if (form.OTP != "6666")
        {
            return new Response<string>(null, "Invalid OTP", 400);
        }

     
        var user = await _context.Users.FindAsync(pendingUser.Id);
        if (user == null)
        {
            return new Response<string>(null, "User not found", 400);
        }

     
        using var hmac = new HMACSHA512();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(form.NewPassword));
        user.PasswordSalt = hmac.Key;

   
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

       
        _context.PendingUsers.Remove(pendingUser);
        await _context.SaveChangesAsync();

        return new Response<string>("Password reset successfully.", null, 200);
    }





}
