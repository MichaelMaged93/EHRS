using EHRS.Api.Services;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Auth;
using EHRS.Core.Requests.DoctorAuth;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DoctorAuthController : ControllerBase
{
    private readonly IDoctorAuthQueries _queries;
    private readonly JwtTokenService _jwt;

    public DoctorAuthController(IDoctorAuthQueries queries, JwtTokenService jwt)
    {
        _queries = queries;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DoctorRegisterRequest request)
    {
        var (success, message) = await _queries.RegisterAsync(request);
        if (!success) return BadRequest(new { message });

        // Register للدكتور = Pending → مش هنرجع JWT
        return Ok(new { message });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] DoctorLoginRequest request)
    {
        try
        {
            var user = await _queries.LoginAsync(request);
            var (token, exp) = _jwt.CreateToken(user.UserId, user.Role, user.FullName, user.Email, request.RememberMe);

            return Ok(new AuthTokenDto
            {
                AccessToken = token,
                ExpiresInMinutes = exp,
                User = user
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // Pending / Rejected → 403
            if (ex.Message.Contains("under review", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("rejected", StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, new { message = ex.Message });

            return BadRequest(new { message = ex.Message });
        }
    }
}
