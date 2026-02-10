using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace EHRS.Api.Services;

public sealed class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config) => _config = config;

    public (string Token, int ExpiresInMinutes) CreateToken(
        int userId,
        string role,
        string fullName,
        string? email,
        bool rememberMe)
    {
        var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
        var issuer = _config["Jwt:Issuer"] ?? "EHRS";
        var audience = _config["Jwt:Audience"] ?? "EHRS.Client";

        var expMinutes = int.TryParse(_config["Jwt:ExpMinutes"], out var m) ? m : 60;
        if (rememberMe) expMinutes *= 24;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, role),
            new("userId", userId.ToString()),
            new("fullName", fullName),
        };

        if (!string.IsNullOrWhiteSpace(email))
            claims.Add(new(ClaimTypes.Email, email));

        if (role == "Patient") claims.Add(new("patientId", userId.ToString()));
        if (role == "Doctor") claims.Add(new("doctorId", userId.ToString()));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expMinutes),
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expMinutes);
    }
}
