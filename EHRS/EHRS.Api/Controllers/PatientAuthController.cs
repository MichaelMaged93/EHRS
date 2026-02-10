using EHRS.Api.Services;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Auth;
using EHRS.Core.Requests.PatientAuth;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PatientAuthController : ControllerBase
{
    private readonly IPatientAuthQueries _queries;
    private readonly JwtTokenService _jwt;

    public PatientAuthController(IPatientAuthQueries queries, JwtTokenService jwt)
    {
        _queries = queries;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] PatientRegisterRequest request)
    {
        try
        {
            var user = await _queries.RegisterAsync(request);
            var (token, exp) = _jwt.CreateToken(user.UserId, user.Role, user.FullName, user.Email, rememberMe: false);

            return Ok(new AuthTokenDto
            {
                AccessToken = token,
                ExpiresInMinutes = exp,
                User = user
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] PatientLoginRequest request)
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
            return BadRequest(new { message = ex.Message });
        }
    }
}
