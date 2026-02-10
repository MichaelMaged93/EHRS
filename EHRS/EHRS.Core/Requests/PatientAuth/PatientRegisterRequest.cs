namespace EHRS.Core.Requests.PatientAuth;

public sealed class PatientRegisterRequest
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateOnly BirthDate { get; set; }
    public string Gender { get; set; } = default!;
    public string NationalId { get; set; } = default!; // maps to Patient.Ssn
    public string Password { get; set; } = default!;
    public string ConfirmPassword { get; set; } = default!;
}
