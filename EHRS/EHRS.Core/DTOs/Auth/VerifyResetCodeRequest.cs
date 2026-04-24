namespace EHRS.Core.DTOs.Auth;

public class VerifyResetCodeRequest
{
    public string Email { get; set; }
    public string Token { get; set; }
}