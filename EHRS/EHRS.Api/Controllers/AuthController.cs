using EHRS.Core.DTOs.Auth;
using EHRS.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    // 🔐 Forgot Password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var result = await _auth.ForgotPasswordAsync(request.Email);
        return Ok(result);
    }

    // 🔐 Verify Reset Code
    [HttpPost("verify-reset-code")]
    public async Task<IActionResult> VerifyResetCode(VerifyResetCodeRequest request)
    {
        var result = await _auth.VerifyResetCodeAsync(request.Email, request.Token);
        return Ok(result);
    }

    // 🔐 Reset Password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        if (request.NewPassword != request.ConfirmPassword)
            return BadRequest("Password mismatch");

        var result = await _auth.ResetPasswordAsync(
            request.Email,
            request.Token,
            request.NewPassword);

        return result ? Ok() : BadRequest("Invalid request");
    }

    // 🔐 Change Password (JWT BASED)
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = GetUserIdFromToken();

        if (userId == null)
            return Unauthorized("Invalid token");

        if (request.NewPassword != request.ConfirmPassword)
            return BadRequest("Password mismatch");

        var result = await _auth.ChangePasswordAsync(
            userId.Value,
            request.CurrentPassword,
            request.NewPassword);

        return result ? Ok() : BadRequest("Invalid password");
    }

    // 🔐 Extract userId from JWT (FINAL)
    private int? GetUserIdFromToken()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (int.TryParse(userIdClaim, out int userId))
            return userId;

        return null;
    }
}