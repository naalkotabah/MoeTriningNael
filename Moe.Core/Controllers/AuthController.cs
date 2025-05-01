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
    /// Default accounts (All have "string" as password):
    /// - sa@example.com (Super Admin)
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
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordFormDTO form)
    {
        await _authService.ResetPassword(CurId, form);
        return Ok(new Response<object>(null, null, 200));
    }



    [Authorize]
    [HttpPost("ForgetPassword")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordFormDTO form)
    {
        var response = await _authService.ForgetPassword(form);
        if (response.StatusCode == 200)
        {
            return Ok(response); 
        }
        else
        {
            return BadRequest(response); 
        }
    }

    [Authorize]
    [HttpPost("VerifyOtp")]
    public async Task<IActionResult> VerifyOtp([FromBody] ForgetPasswordVerifyOtpFormDTO form)
    {
        
        var response = await _authService.VerifyForgetPasswordOtp(CurId, form);

        if (response.StatusCode == 200)
        {
            return Ok(response); 
        }
        else
        {
            return BadRequest(response);
        }
    }


    #endregion

    #region OAuth
    #endregion
}
