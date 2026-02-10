namespace EHRS.Core.Requests.DoctorAuth;

public sealed class DoctorRegisterRequest
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string MedicalLicense { get; set; } = default!;
    public string Specialization { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
}
