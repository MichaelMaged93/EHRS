namespace EHRS.Core.DTOs.Auth;

public sealed class AuthTokenDto
{
    public string AccessToken { get; set; } = default!;
    public int ExpiresInMinutes { get; set; }
    public AuthUserDto User { get; set; } = new();
}
