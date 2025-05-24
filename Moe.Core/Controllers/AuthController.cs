using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moe.Core.ActionFilters;
using Moe.Core.Helpers;
using Moe.Core.Models.DTOs.Auth;
using Moe.Core.Services;
using Moe.Core.Data;
using Moe.Core.Models.DTOs.User;
using Moe.Core.Models.Entities;
using Moe.Core.Models.Entities.Moe.Core.Models.Entities;

namespace Moe.Core.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly PhoneNumberNormalizer _phoneNumberNormalizer;

    public AuthController(IAuthService authService, PhoneNumberNormalizer phoneNumberNormalizer)
    {
        _authService = authService;
        _phoneNumberNormalizer = phoneNumberNormalizer;
    }

    #region Register
    [HttpPost("register")]
    public async Task<ActionResult<Response<string>>> Register([FromBody] RegisterFormDTO form)
    {
        form.Phone = _phoneNumberNormalizer.Normalize(form.Phone);
        form.PhoneCountryCode = _phoneNumberNormalizer.NormalizeCountryCode(form.PhoneCountryCode);
        return Ok(await _authService.Register(form));
    }
    
    [HttpPost("verify-register")]
    public async Task<ActionResult<Response<string>>> VerifyRegister([FromBody] VerifyOtpFormDTO form) =>
        Ok(await _authService.VerifyRegister(form));
    #endregion

    #region Auth
    /// <summary>
    /// Handles user login
    /// </summary>
    /// <remarks>
    ///  Default test accounts (all use password: <b>string</b>):
    /// 
    /// -  Super Admin:
    ///   Email: <c>superadmin@example.com</c>
    ///   Phone: <c>+964700000001</c>
    ///
    /// -  Admin:
    ///   Email: <c>admin@example.com</c>
    ///   Phone: <c>+964700000002</c>
    ///
    /// -  Normal User:
    ///   Email: <c>normal@example.com</c>
    ///   Phone: <c>+964700000003</c>
    ///
    /// You can login using either email or phone + country code.
    /// </remarks>

    [HttpPost("login")]
    public async Task<ActionResult<Response<string>>> Login([FromBody] LoginFormDTO form)
    {
        form.Phone = _phoneNumberNormalizer.Normalize(form.Phone);
        form.PhoneCountryCode = _phoneNumberNormalizer.NormalizeCountryCode(form.PhoneCountryCode);
        return Ok(await _authService.Login(form));
    }
    /// <summary>
    /// Reset your password
    /// </summary>
    /// <remarks>
    /// Required Roles: `Any`
    /// </remarks>
    [Authorize]
    [HttpPost("reset-password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<Response<string>>> ResetPassword([FromBody] ResetPasswordFormDTO form)
    {
        form.UserId = CurId; 

        var result = await _authService.ResetPassword(form);
            return Ok(result); 
       
    }

    [HttpPost("change-password")]
    public async Task<ActionResult<Response<string>>> RequestChangePassword([FromBody] ChangePasswordRequestFormDTO form)
    {
        var result = await _authService.CreateChangePasswordRequest(form);
        return Ok(result);
    }
    [HttpPost("change-password/verify")]
    public async Task<ActionResult<Response<string>>> VerifyChangePassword([FromBody] ChangePasswordRequestVerificationFormDTO form)
    {

        var result = await _authService.VerifyChangePasswordRequest(form);
        return Ok(result);
    }


    [Authorize]
    [HttpPost("change-email")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Response<string>>> RequestChangeEmail([FromBody] ChangeEmailRequestFormDTO form)
    {
        form.UserId = CurId; 
        var result = await _authService.CreateChangeEmailRequest(form);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("change-email/verify")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Response<string>>> VerifyChangeEmail([FromBody] ChangeEmailRequestVerificationFormDTO form)
    {
        form.CurId = CurId; 
        var result = await _authService.VerifyChangeEmailRequest(form);
        return Ok(result);
    }
    [Authorize]
    [HttpPost("change-Phone")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Response<string>>> RequestPhoneEmail([FromBody] ChangePhoneFormDTO form)
    {
        form.UserId = CurId;
        var result = await _authService.CreateChangePhoneRequest(form);
        return Ok(result);
    }
    [Authorize]
    [HttpPost("change-Phone/verify")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<Response<string>>> VerifyChangePhone([FromBody] ChangePhoneRequestVerificationFormDTO form)
    {
        form.CurId = CurId;
        var result = await _authService.VerifyChangePhoneRequest(form);
        return Ok(result);
    }
    #endregion

    #region OAuth
    #endregion
}
