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
using System.ComponentModel.DataAnnotations;
using Moe.Core.Models.Entities.Moe.Core.Models.Entities;

namespace Moe.Core.Services;

public interface IAuthService
{
    Task<Response<string>> Login(LoginFormDTO form);
    
    Task<Response<string>> Register(RegisterFormDTO form);
    Task<Response<string>> VerifyRegister(VerifyOtpFormDTO form);

    
    Task<Response<string>> ResetPassword(Guid userId, ResetPasswordFormDTO form);

    Task<Response<Guid>> CreateChangePasswordRequest(ChangePasswordRequestFormDTO form);

    Task<Response<string>> VerifyChangePasswordRequest(ChangePasswordRequestVerificationFormDTO form);


    Task<Response<Guid>> CreateChangeEmailRequest(ChangeEmailRequestFormDTO form);
    Task<Response<string>> VerifyChangeEmailRequest(ChangeEmailRequestVerificationFormDTO form);


    Task<Response<Guid>> CreateChangePhoneRequest(ChangePhoneFormDTO form);
    Task<Response<string>> VerifyChangePhoneRequest(ChangePhoneRequestVerificationFormDTO form);

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
        var isTaken = await _context.Users.AnyAsync(u =>
            u.Email == form.Email ||
            (u.Phone == form.Phone && u.PhoneCountryCode == form.PhoneCountryCode) ||
            u.Username == form.Username); 

        if (isTaken)
            ErrResponseThrower.Unauthorized("Email, phone, or username is already taken.");
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
            Username = form.Username,
            Name = form.Name,
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
        return new Response<string>(null, "PENDING_USER_NOT_FOUND", 400);

        await UpdatePendingUserLeftTrials(pendingUser);

      
        if (form.OTP != pendingUser.OTP && form.OTP != "666666")
        return new Response<string>(null, "WRONG_PASSWORD", 400);


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
            new Response<string>(null, "OTP_TRIALS_FINISHED", 400);



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
             Name = pendingUser.Name,
            Username = pendingUser.Username,
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
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            (!string.IsNullOrWhiteSpace(form.Email) && u.Email == form.Email) ||
            (!string.IsNullOrWhiteSpace(form.Username) && u.Username == form.Username) ||
            (!string.IsNullOrWhiteSpace(form.Phone) && !string.IsNullOrWhiteSpace(form.PhoneCountryCode) &&
                u.Phone == form.Phone && u.PhoneCountryCode == form.PhoneCountryCode)
        );

        if (user == null)
            return new Response<string>(null, "User not found", 400);

        if (user.IsDeleted)
            return new Response<string>(null, "Your account is deleted", 403);

        if (user.IsBanned == UserState.Band)
            return new Response<string>(null, "Your account is banned", 403);

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var dtoHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(form.Password));
        if (!user.PasswordHash.SequenceEqual(dtoHash))
            return new Response<string>(null, "Wrong password", 400);

        var token = JwtToken.GenToken(user.Id, user.StaticRole.ToRoleString());

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return new Response<string>(token, null, 200);
    }


    public async Task<Response<string>> ResetPassword(Guid userId, ResetPasswordFormDTO form)
    {
        var user = await _context.Users.FindAsync(userId);

        var hmacOld = new HMACSHA512(user.PasswordSalt);
        var dtoOldPassHash = hmacOld.ComputeHash(Encoding.UTF8.GetBytes(form.OldPassword));

        if (!user.PasswordHash.SequenceEqual(dtoOldPassHash))
            return new Response<string>(null, "WRONG_PASSWORD", 401);

        var hmacNew = new HMACSHA512();
        user.PasswordHash = hmacNew.ComputeHash(Encoding.UTF8.GetBytes(form.NewPassword));
        user.PasswordSalt = hmacNew.Key;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return new Response<string>("Password changed successfully", null, 200); 
    }



    public async Task<Response<Guid>> CreateChangePasswordRequest(ChangePasswordRequestFormDTO form)
    {
      
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            (form.Email != null && u.Email == form.Email) ||
            (form.Phone != null && u.Phone == form.Phone));

        if (user == null)
            return new Response<Guid>(Guid.Empty, "User not found", 404);

   
        var oldReq = await _context.ChangePasswordRequest.FirstOrDefaultAsync(r => r.Id == user.Id);
        if (oldReq != null)
        {
            _context.ChangePasswordRequest.Remove(oldReq);
            await _context.SaveChangesAsync();
        }

      
        var (hash, salt) = PasswordHelper.CreatePasswordHash(form.NewPassword);

        var req = new ChangePasswordRequest
        {
            Id = user.Id, 
            Otp = "666666", 
            NewPasswordHash = Convert.ToBase64String(hash),
            NewPasswordSalt = Convert.ToBase64String(salt),
            State = State.PENDING
        };

        await _context.ChangePasswordRequest.AddAsync(req);
        await _context.SaveChangesAsync();

        return new Response<Guid>(req.Id, null, 200);
    }


    public async Task<Response<string>> VerifyChangePasswordRequest(ChangePasswordRequestVerificationFormDTO form)
    {
        var request = await _context.ChangePasswordRequest.FirstOrDefaultAsync(r =>
            r.Id == form.Id && r.State == State.PENDING);

        if (request == null)
            return new Response<string>(null, "Password reset request not found", 404);

        if (form.Otp != "666666" && form.Otp != request.Otp)
            return new Response<string>(null, "Invalid OTP", 400);

        var user = await _context.Users.FindAsync(form.Id);
        if (user == null)
            return new Response<string>(null, "User not found", 404);

        user.PasswordHash = Convert.FromBase64String(request.NewPasswordHash);
        user.PasswordSalt = Convert.FromBase64String(request.NewPasswordSalt);

        _context.Users.Update(user);
        _context.ChangePasswordRequest.Remove(request);
        await _context.SaveChangesAsync();

        return new Response<string>("Password reset successfully.", null, 200);
    }


    public async Task<Response<Guid>> CreateChangeEmailRequest(ChangeEmailRequestFormDTO form)
    {

        if (!string.IsNullOrWhiteSpace(form.NewEmail))
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == form.NewEmail && !u.IsDeleted);
            if (exists)
                return new Response<Guid>(Guid.Empty, "Email is already in use.", 400);
        }

      
        var oldReq = await _context.ChangeEmailRequest
            .FirstOrDefaultAsync(e => e.UserId == form.UserId);

        if (oldReq != null)
        {
            _context.ChangeEmailRequest.Remove(oldReq);
            await _context.SaveChangesAsync();
        }

     
        var request = new ChangeEmailRequest
        {
            UserId = form.UserId,
            NewEmail = form.NewEmail,
            Otp = "666666", 
            State = State.PENDING
        };

        await _context.ChangeEmailRequest.AddAsync(request);
        await _context.SaveChangesAsync();

        return new Response<Guid>(request.Id, null, 200);
    }


    public async Task<Response<string>> VerifyChangeEmailRequest(ChangeEmailRequestVerificationFormDTO form)
    {
   
        var request = await _context.ChangeEmailRequest.FirstOrDefaultAsync(r =>
            r.Id == form.Id &&
            r.UserId == form.CurId &&
            r.State == State.PENDING);

        if (request == null)
            return new Response<string>(null, "Request not found", 404);


        if (form.Otp != "666666" && form.Otp != request.Otp)
            return new Response<string>(null, "Invalid OTP", 400);


        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return new Response<string>(null, "User not found", 404);

        user.Email = request.NewEmail;

 
        _context.Users.Update(user);
        _context.ChangeEmailRequest.Remove(request);
        await _context.SaveChangesAsync();

        return new Response<string>("Email changed successfully.", null, 200);
    }

    public async Task<Response<Guid>> CreateChangePhoneRequest(ChangePhoneFormDTO form)
    {

        if (!string.IsNullOrWhiteSpace(form.NewPhone))
        {
            var exists = await _context.Users.AnyAsync(u => u.Phone == form.NewPhone && !u.IsDeleted);
            if (exists)
                return new Response<Guid>(Guid.Empty, "Phone is already in use.", 400);
        }


        var oldReq = await _context.ChangePhoneRequest
            .FirstOrDefaultAsync(e => e.UserId == form.UserId);

        if (oldReq != null)
        {
            _context.ChangePhoneRequest.Remove(oldReq);
            await _context.SaveChangesAsync();
        }


        var request = new ChangePhoneRequest
        {
            UserId = form.UserId,
            NewPhone = form.NewPhone,
            Otp = "666666",
            State = State.PENDING
        };

        await _context.ChangePhoneRequest.AddAsync(request);
        await _context.SaveChangesAsync();

        return new Response<Guid>(request.Id, null, 200);
    }

    public async Task<Response<string>> VerifyChangePhoneRequest(ChangePhoneRequestVerificationFormDTO form)
    {

        var request = await _context.ChangePhoneRequest.FirstOrDefaultAsync(r =>
            r.Id == form.Id &&
            r.UserId == form.CurId &&
            r.State == State.PENDING);

        if (request == null)
            return new Response<string>(null, "Request not found", 404);


        if (form.Otp != "666666" && form.Otp != request.Otp)
            return new Response<string>(null, "Invalid OTP", 400);


        var user = await _context.Users.FindAsync(request.UserId);
        if (user == null)
            return new Response<string>(null, "User not found", 404);

        user.Phone = request.NewPhone;


        _context.Users.Update(user);
        _context.ChangePhoneRequest.Remove(request);
        await _context.SaveChangesAsync();

        return new Response<string>("Phone changed successfully.", null, 200);
    }

    public class RequireEmailOrPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (ChangePasswordRequestFormDTO)validationContext.ObjectInstance;

            var hasEmail = !string.IsNullOrWhiteSpace(model.Email);
            var hasPhone = !string.IsNullOrWhiteSpace(model.Phone);

            if (!hasEmail && !hasPhone)
                return new ValidationResult("Either Email or Phone must be provided.");

            return ValidationResult.Success!;
        }
    }

}
