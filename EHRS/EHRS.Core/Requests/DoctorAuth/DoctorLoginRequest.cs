namespace EHRS.Core.Requests.DoctorAuth;

public sealed class DoctorLoginRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool RememberMe { get; set; } = false;
}
