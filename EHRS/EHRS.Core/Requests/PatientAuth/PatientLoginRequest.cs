namespace EHRS.Core.Requests.PatientAuth;

public sealed class PatientLoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool RememberMe { get; set; } = false;
}
